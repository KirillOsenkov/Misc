using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Number = System.Double;

namespace WriteableBitmapExperiment
{
    public class MainWindow : Window
    {
        WriteableBitmap bitmap;
        Number moveFactor = 0.1f;
        Number scaleFactor = 1.5f;
        Number width;
        Number height;
        Number logicalWidth = 2;
        Number logicalHeight = 2;
        Number leftBound = 0;
        Number topBound = 0;
        Number xstep;
        private int alpha = 255 << 24;

        [STAThread]
        static void Main(string[] args)
        {
            var window = new MainWindow();
            var app = new Application();
            app.Run(window);
        }

        private Image image;

        public MainWindow()
        {
            Title = "MainWindow";
            Height = 600;
            Width = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            KeyDown += Window_KeyDown;

            image = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Stretch = Stretch.None,
                VerticalAlignment = VerticalAlignment.Center
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            Content = new Border()
            {
                Background = Brushes.LightGray,
                Child = image
            };

            width = 600;
            height = 600;
            bitmap = BitmapFactory.New((int)width, (int)height);
            Update();
            image.Source = bitmap;
        }

        private void Update()
        {
            Stopwatch sw = Stopwatch.StartNew();

            unsafe
            {
                xstep = logicalWidth / width;
                var ystep = logicalHeight / height;
                bitmap.Lock();

                int* backBuffer = (int*)bitmap.BackBuffer;
                int length = bitmap.PixelWidth * bitmap.PixelHeight;
                Number rightBound = leftBound + logicalWidth;
                Number bottomBound = topBound + logicalHeight;

                var degreeOfParallelism = Environment.ProcessorCount;
                var tasks = new Task[degreeOfParallelism];

                for (int core = 0; core < degreeOfParallelism; core++)
                {
                    int currentCore = core;
                    int* localBackBuffer = backBuffer;
                    tasks[core] = Task.Run(() =>
                    {
                        int j;
                        Number y;
                        for (y = topBound, j = 0; j < height; y += ystep * degreeOfParallelism, j += degreeOfParallelism)
                        {
                            ComputeRow(localBackBuffer + currentCore * (int)width, y + currentCore * ystep);
                            localBackBuffer += degreeOfParallelism * (int)width;
                        }
                    });
                }

                Task.WaitAll(tasks);
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, (int)width, (int)height));
            bitmap.Unlock();

            Title = "Elapsed: " + sw.Elapsed;
        }

        unsafe private void ComputeRow(int* backBuffer, Number y)
        {
            int i;
            Number x;
            for (x = leftBound, i = 0; i < width; x += xstep, i++)
            {
                *(backBuffer + i) = GetColorInt(x, y);
            }
        }

        private int componentsX = 2;
        private int componentsY = 2;

        Number[,] coefficientsR = new Number[,] { { 0.226965873510098, 0.0809162576230849 }, { -0.00505726610144281, -0.0809162576230849 } };
        Number[,] coefficientsG = new Number[,] { { 0.208636870145256, -0.0455153949129853 }, { 0.0809162576230849, -0.0316079131340176 } };
        Number[,] coefficientsB = new Number[,] { { 0.313988713375718, 0.0619515097426744 }, { 0.102409638554217, 0.0619515097426744 } };

        private int GetColorInt(
            Number x, Number y)
        {
            var r = 0.0;
            var g = 0.0;
            var b = 0.0;

            for (var j = 0; j < componentsY; j++)
            {
                for (var i = 0; i < componentsX; i++)
                {
                    var basis = Math.Cos((Math.PI * x * i) / logicalWidth) * Math.Cos((Math.PI * y * j) / logicalHeight);
                    r += coefficientsR[i, j] * basis;
                    g += coefficientsG[i, j] * basis;
                    b += coefficientsB[i, j] * basis;
                }
            }

            int intR = LinearTosRgb(r);
            int intG = LinearTosRgb(g);
            int intB = LinearTosRgb(b);

            int color_data = intR << 16;
            color_data |= intG << 8;
            color_data |= intB << 0;
            color_data |= alpha;
            return color_data;
        }

        /// <summary>
        /// Converts a linear double value into an sRGB input value (0 to 255)
        /// </summary>
        public static int LinearTosRgb(double value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1.0)
            {
                value = 1.0;
            }

            var v = value;
            if (v <= 0.0031308) return (int)(v * 12.92 * 255 + 0.5);
            else return (int)((1.055 * Math.Pow(v, 1 / 2.4) - 0.055) * 255 + 0.5);
        }

        private int GetColorInt2(Number x, Number y)
        {
            var color = GetColor(x, y) * 20;

            int r = color % 256;
            int g = 0;
            int b = 0;

            if (color > 255)
            {
                g = (color - 256) % 256;
                if (color > 512)
                {
                    b = (color - 512) % 256;
                }
            }

            int color_data = r << 16;
            color_data |= g << 8;
            color_data |= b << 0;
            color_data |= alpha;
            return color_data;
        }

        private int GetColor(Number x0, Number y0)
        {
            Number x = x0;
            Number y = y0;
            for (int i = 0; i < 300; i++)
            {
                var x2 = x * x;
                var y2 = y * y;
                var sx = x2 - y2;
                var sy = 2 * x * y;
                x = sx + x0;
                y = sy + y0;

                if (x2 + y2 > 4)
                {
                    return i;
                }
            }

            return 0;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home || e.Key == Key.PageUp || e.Key == Key.OemPlus)
            {
                ZoomIn();
            }
            else if (e.Key == Key.End || e.Key == Key.PageDown || e.Key == Key.OemMinus)
            {
                ZoomOut();
            }
            else if (e.Key == Key.Left)
            {
                Left();
            }
            else if (e.Key == Key.Right)
            {
                Right();
            }
            else if (e.Key == Key.Up)
            {
                Up();
            }
            else if (e.Key == Key.Down)
            {
                Down();
            }
            else
            {
                Close();
            }
        }

        private void Down()
        {
            this.topBound += logicalHeight * moveFactor;
            Update();
        }

        private void Up()
        {
            this.topBound -= logicalHeight * moveFactor;
            Update();
        }

        private void Right()
        {
            this.leftBound += logicalWidth * moveFactor;
            Update();
        }

        private void Left()
        {
            this.leftBound -= logicalWidth * moveFactor;
            Update();
        }

        private void ZoomOut()
        {
            this.logicalWidth = logicalWidth * scaleFactor;
            this.logicalHeight = logicalHeight * scaleFactor;
            this.leftBound = leftBound - (logicalWidth - logicalWidth / scaleFactor) / 2;
            this.topBound = topBound - (logicalHeight - logicalHeight / scaleFactor) / 2;
            Update();
        }

        private void ZoomIn()
        {
            this.leftBound = leftBound + (logicalWidth - logicalWidth / scaleFactor) / 2;
            this.topBound = topBound + (logicalHeight - logicalHeight / scaleFactor) / 2;
            this.logicalWidth = logicalWidth / scaleFactor;
            this.logicalHeight = logicalHeight / scaleFactor;
            Update();
        }
    }

    public struct Complex
    {
        public Number X;
        public Number Y;

        public Complex(Number x, Number y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.X + b.X, a.Y + b.Y);
        }

        public Number DistanceSquare(Complex other)
        {
            return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y);
        }

        public Number ModuleSquare
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public Complex Square
        {
            get
            {
                return new Complex(X * X - Y * Y, 2 * X * Y);
            }
        }
    }
}
