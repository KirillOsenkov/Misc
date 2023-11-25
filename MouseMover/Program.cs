using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace MouseMover
{
    class Program
    {
        static TimeSpan interval = TimeSpan.FromSeconds(5);
        private const int delta = 1000;

        [STAThread]
        static void Main(string[] args)
        {
            var app = new System.Windows.Application();
            var window = new Window();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Width = 300;
            window.Height = 300;
            window.Title = "Mouse Mover";
            window.Loaded += Window_Loaded;
            app.Run(window);
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = interval;
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        static Point previousPosition;
        static bool wasLeft = false;
        private static DispatcherTimer timer;

        private static void Timer_Tick(object sender, EventArgs e)
        {
            var position = GetCursorPosition();
            if (position == previousPosition)
            {
                position = IncrementPosition(position);
                SendMouseEvent((uint)position.X, (uint)position.Y);
            }

            previousPosition = position;
        }

        private static Point IncrementPosition(Point position)
        {
            int currentDelta = wasLeft ? delta : -delta;
            wasLeft = !wasLeft;
            return new Point(position.X + currentDelta, position.Y + Math.Sign(currentDelta) * 100);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("CoreDll.dll")]
        public static extern void SystemIdleTimerReset();

        public static void SendMouseEvent(uint screenX, uint screenY)
        {
            //SystemIdleTimerReset();

            SetCursorPos((int)screenX, (int)screenY);

            // Convert the device-units into normalized screen space.
            // We find the center-point of the pixel in normalized screen
            // space by finding the left/right/top/bottom edges and
            // calculating the middle.
            var _rcScreen = Screen.PrimaryScreen.Bounds;
            double normalizedX1 = (65535.0 * (screenX)) / ((double)_rcScreen.Width);
            double normalizedY1 = (65535.0 * (screenY)) / ((double)_rcScreen.Height);
            double normalizedX2 = (65535.0 * (screenX + 1)) / ((double)_rcScreen.Width);
            double normalizedY2 = (65535.0 * (screenY + 1)) / ((double)_rcScreen.Height);

            int normalizedX = (int)(normalizedX1 + ((normalizedX2 - normalizedX1) / 2.0));
            int normalizedY = (int)(normalizedY1 + ((normalizedY2 - normalizedY1) / 2.0));

            mouse_event((int)MouseFlags.MOUSEEVENTF_MOVE | (int)MouseFlags.MOUSEEVENTF_ABSOLUTE, (int)(normalizedX), (int)(normalizedY), 0, 0);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        public enum MouseFlags
        {
            MOUSEEVENTF_ABSOLUTE = 0x8000,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100
        }

        public enum DataFlags
        {
            XBUTTON1 = 0x0001,
            XBUTTON2 = 0x0002
        }
    }
}
