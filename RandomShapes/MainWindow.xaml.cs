using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RandomShapes
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseUp += MainWindow_MouseUp;
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Generate();
        }

        private void Generate()
        {
            //canvas.Children.Clear();

            int howMany = 1;

            for (int i = 0; i < howMany; i++)
            {
                var canvasWidth = canvas.ActualWidth;
                var canvasHeight = canvas.ActualHeight;

                double left = Random * canvasWidth;
                double top = Random * canvasHeight;
                double width = (canvasWidth - left) * Random;
                double height = (canvasHeight - top) * Random;
                double angle = Random * 90;

                var transform = new RotateTransform(Random * 90);

                Color color = GetRandomColor();
                Color color2 = GetRandomColor();

                var rectangle = new Rectangle();
                rectangle.LayoutTransform = transform;
                rectangle.Fill = new LinearGradientBrush(color, color2, angle);
                Canvas.SetLeft(rectangle, left);
                Canvas.SetTop(rectangle, top);
                rectangle.Width = width;
                rectangle.Height = height;

                canvas.Children.Add(rectangle);
            }
        }

        private Color GetRandomColor()
        {
            byte transparency = (byte)(Random * 255);
            byte red = (byte)(Random * 255);
            byte green = (byte)(Random * 255);
            byte blue = (byte)(Random * 255);
            Color color = Color.FromArgb(transparency, red, green, blue);
            return color;
        }

        Random random = new Random();
        double Random => random.NextDouble();
    }
}
