using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColoringState
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            colorPicker.Color = Colors.YellowGreen;
        }

        public double Thickness = 200;

        Point startPosition;
        Point endPosition;
        Line line;

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPosition = e.GetPosition(canvas);
            line = new Line();
            line.Stroke = new SolidColorBrush(Color.FromArgb(100, colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
            line.StrokeThickness = Thickness;
            line.X1 = startPosition.X;
            line.Y1 = startPosition.Y;
            line.X2 = line.X1;
            line.Y2 = line.Y1;
            canvas.Children.Add(line);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            endPosition = e.GetPosition(canvas);
            if (line != null)
            {
                line.X2 = endPosition.X;
                line.Y2 = endPosition.Y;
            }
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            line = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Delete all?", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            canvas.Children.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
        }
    }
}
