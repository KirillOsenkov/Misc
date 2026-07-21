// Minimal-latency drag test: D3D11 flip-model swapchain, waitable object,
// max frame latency 1, optional vsync-off with tearing.
// Keys: V = toggle vsync, C = custom-rendered cursor (hides the hardware cursor
// and draws a crosshair in-frame, so cursor and content share the same latency),
// F11 = borderless fullscreen, Esc = quit. Drag anywhere; the square follows the cursor.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;

static class Native
{
    [DllImport("user32.dll")] public static extern bool GetCursorPos(out POINT p);
    [DllImport("user32.dll")] public static extern bool ScreenToClient(IntPtr hwnd, ref POINT p);
    [DllImport("kernel32.dll")] public static extern uint WaitForSingleObject(IntPtr h, uint ms);
    public struct POINT { public int X, Y; }
}

class DragTestForm : Form
{
    ID3D11Device _device;
    ID3D11DeviceContext _context;
    ID3D11DeviceContext1 _context1;
    IDXGISwapChain1 _swapChain;
    IDXGISwapChain2 _swapChain2;
    ID3D11RenderTargetView _rtv;
    IntPtr _frameLatencyWaitable;
    readonly object _resizeLock = new object();
    volatile bool _needResize;
    volatile bool _vsync = true;
    volatile bool _customCursor;
    volatile bool _running = true;
    bool _allowTearing;
    string _adapterName = "?";

    public DragTestForm()
    {
        Text = "DragTest";
        ClientSize = new System.Drawing.Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        CreateDeviceAndSwapChain();
        var thread = new Thread(RenderLoop) { IsBackground = true };
        thread.Start();
    }

    void CreateDeviceAndSwapChain()
    {
        D3D11.D3D11CreateDevice(null, DriverType.Hardware, DeviceCreationFlags.BgraSupport,
            new[] { FeatureLevel.Level_11_1, FeatureLevel.Level_11_0 },
            out _device, out _context).CheckError();
        _context1 = _context.QueryInterface<ID3D11DeviceContext1>();

        using var dxgiDevice = _device.QueryInterface<IDXGIDevice>();
        using var adapter = dxgiDevice.GetAdapter();
        _adapterName = adapter.Description.Description;
        using var factory = adapter.GetParent<IDXGIFactory2>();

        using (var factory5 = factory.QueryInterfaceOrNull<IDXGIFactory5>())
            _allowTearing = factory5 != null && factory5.PresentAllowTearing;

        var flags = SwapChainFlags.FrameLatencyWaitableObject;
        if (_allowTearing) flags |= SwapChainFlags.AllowTearing;

        var desc = new SwapChainDescription1
        {
            Width = 0,
            Height = 0,
            Format = Format.B8G8R8A8_UNorm,
            Stereo = false,
            SampleDescription = new SampleDescription(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = 2,
            Scaling = Scaling.None,
            SwapEffect = SwapEffect.FlipDiscard,
            AlphaMode = AlphaMode.Ignore,
            Flags = flags,
        };
        _swapChain = factory.CreateSwapChainForHwnd(_device, Handle, desc);
        factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAltEnter);

        _swapChain2 = _swapChain.QueryInterface<IDXGISwapChain2>();
        _swapChain2.MaximumFrameLatency = 1;
        _frameLatencyWaitable = _swapChain2.FrameLatencyWaitableObject;

        CreateRtv();
    }

    void CreateRtv()
    {
        using var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0);
        _rtv = _device.CreateRenderTargetView(backBuffer);
    }

    void RenderLoop()
    {
        var sw = Stopwatch.StartNew();
        int frames = 0;
        double lastTitleUpdate = 0;

        while (_running)
        {
            Native.WaitForSingleObject(_frameLatencyWaitable, 1000);
            if (!_running) break;

            lock (_resizeLock)
            {
                if (_needResize)
                {
                    _rtv?.Dispose();
                    _rtv = null;
                    var flags = SwapChainFlags.FrameLatencyWaitableObject;
                    if (_allowTearing) flags |= SwapChainFlags.AllowTearing;
                    _swapChain.ResizeBuffers(2, 0, 0, Format.B8G8R8A8_UNorm, flags);
                    CreateRtv();
                    _needResize = false;
                }

                // Sample the cursor as late as possible: right before we draw.
                Native.GetCursorPos(out var p);
                Native.ScreenToClient(Handle, ref p);

                _context.OMSetRenderTargets(_rtv);
                _context.ClearRenderTargetView(_rtv, new Color4(0.12f, 0.12f, 0.14f, 1f));

                const int half = 40;
                var rect = new Vortice.RawRect(p.X - half, p.Y - half, p.X + half, p.Y + half);
                _context1.ClearView(_rtv, new Color4(1f, 0.55f, 0f, 1f), new[] { rect });

                if (_customCursor)
                {
                    // Crosshair drawn in the same frame as the square: they can
                    // never separate, so the drag feels glued even at 60 Hz.
                    const int arm = 16;
                    var white = new Color4(1f, 1f, 1f, 1f);
                    _context1.ClearView(_rtv, white, new[] { new Vortice.RawRect(p.X - arm, p.Y - 1, p.X + arm, p.Y + 2) });
                    _context1.ClearView(_rtv, white, new[] { new Vortice.RawRect(p.X - 1, p.Y - arm, p.X + 2, p.Y + arm) });
                }

                if (_vsync)
                    _swapChain.Present(1, PresentFlags.None);
                else
                    _swapChain.Present(0, _allowTearing ? PresentFlags.AllowTearing : PresentFlags.None);
            }

            frames++;
            var now = sw.Elapsed.TotalSeconds;
            if (now - lastTitleUpdate >= 0.5)
            {
                var fps = frames / (now - lastTitleUpdate);
                frames = 0;
                lastTitleUpdate = now;
                var mode = _vsync ? "vsync ON" : (_allowTearing ? "vsync OFF + tearing" : "vsync OFF");
                var cur = _customCursor ? "custom cursor" : "system cursor";
                var title = $"DragTest — {_adapterName} — {mode} — {cur} — {fps:F0} fps — [V] vsync, [C] cursor, [F11] fullscreen, [Esc] quit";
                try { BeginInvoke(() => Text = title); } catch { }
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_swapChain != null && ClientSize.Width > 0 && ClientSize.Height > 0)
            _needResize = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.V) _vsync = !_vsync;
        if (e.KeyCode == Keys.C)
        {
            _customCursor = !_customCursor;
            if (_customCursor) Cursor.Hide(); else Cursor.Show();
        }
        if (e.KeyCode == Keys.F11) ToggleFullscreen();
        if (e.KeyCode == Keys.Escape) Close();
    }

    bool _fullscreen;
    FormWindowState _savedState;

    void ToggleFullscreen()
    {
        // Borderless window covering the whole monitor: lets DWM hand the
        // swapchain independent-flip (true tearing / sub-frame latency).
        _fullscreen = !_fullscreen;
        if (_fullscreen)
        {
            _savedState = WindowState;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            Bounds = Screen.FromControl(this).Bounds;
        }
        else
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = _savedState;
            ClientSize = new System.Drawing.Size(1200, 800);
            CenterToScreen();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _running = false;
        if (_customCursor) Cursor.Show();
        base.OnFormClosed(e);
    }

    protected override void OnPaintBackground(PaintEventArgs e) { } // D3D owns the surface
}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new DragTestForm());
    }
}
