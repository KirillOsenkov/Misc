using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace MouseMover
{
    class Program
    {
        static TimeSpan interval = TimeSpan.FromSeconds(30);
        private const int delta = 1;

        [STAThread]
        static void Main(string[] args)
        {
            var app = new Application();
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
            var timer = new DispatcherTimer();
            timer.Interval = interval;
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        static Point previousPosition;
        static bool wasLeft = false;

        private static void Timer_Tick(object sender, EventArgs e)
        {
            var position = GetCursorPosition();
            if (position == previousPosition)
            {
                position = IncrementPosition(position);
                SetCursorPos((int)position.X, (int)position.Y);
            }

            previousPosition = position;
        }

        private static Point IncrementPosition(Point position)
        {
            int currentDelta = wasLeft ? delta : -delta;
            wasLeft = !wasLeft;
            return new Point(position.X + currentDelta, position.Y);
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
    }
}
