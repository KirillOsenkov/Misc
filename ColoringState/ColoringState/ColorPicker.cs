using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static System.Windows.Media.Colors;

namespace ColoringState
{
    public class ColorPicker : UserControl
    {
        public event Action<Color> SelectedColorChanged;
        private TextBox text;

        public ColorPicker()
        {
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Content = grid;

            sample = new Border()
            {
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            text = new TextBox()
            {
                IsReadOnly = true,
                BorderThickness = new Thickness(),
            };

            var swatches = GetSwatches(c =>
            {
                Color = c;
            });
            grid.Children.Add(swatches);
            grid.Children.Add(sample);
            grid.Children.Add(text);
            Grid.SetRow(sample, 1);
            Grid.SetRow(text, 2);

            Color = Colors.White;
        }

        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }

            set
            {
                if (Color == value)
                {
                    return;
                }

                SetValue(ColorProperty, value);
                sample.Background = new SolidColorBrush(value);
                string colorText = value.ToString();
                if (nameLookup.ContainsKey(colorText))
                {
                    text.Text = nameLookup[colorText] + " " + colorText;
                }
                else
                {
                    text.Text = colorText;
                }

                SelectedColorChanged?.Invoke(value);
            }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.White));

        public static Color[,] swatches;

        public const int SwatchSize = 16;

        public static FrameworkElement GetSwatches(Action<Color> colorSink)
        {
            swatches = new Color[10, 14]
            {
                { Black, DarkSlateGray, DarkGreen, Green, Olive, DarkOliveGreen, SeaGreen, DarkSeaGreen, LimeGreen, YellowGreen, MediumAquamarine, LightSeaGreen, DarkCyan, Teal },
                { DimGray, SlateGray, LightSlateGray, ForestGreen, DarkKhaki, OliveDrab, MediumSeaGreen, Chartreuse, LightGreen, MediumSpringGreen, Aquamarine, Turquoise, MediumTurquoise, CadetBlue },
                { Gray, DarkGray, Gainsboro, LightGoldenrodYellow, Beige, GreenYellow, LawnGreen, Lime, SpringGreen, Honeydew, PaleTurquoise, LightBlue, DarkTurquoise, SteelBlue },
                { Silver, LightGray, WhiteSmoke, Snow, White, LemonChiffon, PaleGreen, Ivory, FloralWhite, MintCream, LightCyan, Cyan, DeepSkyBlue, CornflowerBlue },
                { Khaki, BlanchedAlmond, Bisque, Cornsilk, Transparent, OldLace, White, Azure, GhostWhite, AliceBlue, PowderBlue, LightSkyBlue, DodgerBlue, MediumBlue },
                { PaleGoldenrod, Wheat, Moccasin, LightYellow, PapayaWhip, PeachPuff, AntiqueWhite, SeaShell, LavenderBlush, Lavender, LightSteelBlue, SkyBlue, RoyalBlue, Blue },
                { Yellow, BurlyWood, NavajoWhite, Orange, Coral, DarkSalmon, LightSalmon, LightPink, MistyRose, Linen, Thistle, MediumSlateBlue, SlateBlue, DarkSlateBlue },
                { Gold, Tan, SandyBrown, DarkOrange, Tomato, Salmon, LightCoral, RosyBrown, Violet, Pink, Plum, BlueViolet, MediumPurple, DarkBlue },
                { Goldenrod, Peru, Chocolate, OrangeRed, Firebrick, Crimson, IndianRed, PaleVioletRed, Orchid, MediumOrchid, DarkViolet, DarkOrchid, Indigo, Navy },
                { DarkGoldenrod, SaddleBrown, Sienna, Red, DarkRed, Maroon, Brown, HotPink, Magenta, DeepPink, MediumVioletRed, DarkMagenta, Purple, MidnightBlue },
            };

            var grid = new UniformGrid()
            {
                Rows = swatches.GetLength(0),
                Columns = swatches.GetLength(1),
            };

            grid.Width = SwatchSize * grid.Columns;
            grid.Height = SwatchSize * grid.Rows;

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    var swatch = GetColorButton(swatches[row, column], colorSink);
                    Grid.SetColumn(swatch, column);
                    Grid.SetRow(swatch, row);
                    grid.Children.Add(swatch);
                }
            }

            return grid;
        }

        private static FrameworkElement GetColorButton(Color color, Action<Color> colorSink)
        {
            var brush = new SolidColorBrush(color);
            var button = new Button();
            button.Width = SwatchSize;
            button.Height = SwatchSize;
            button.BorderThickness = new Thickness();
            //button.Content = new Border() { Background = brush };
            button.Background = brush;
            button.ToolTip = nameLookup[color.ToString()] + "\r\n" + color.ToString();
            button.Click += (s, e) => colorSink(color);
            return button;
        }

        private static TextBox GetTextBoxSwatch(int row, int column)
        {
            var swatch = new TextBox()
            {
                Background = new SolidColorBrush(swatches[row, column]),
                Tag = Tuple.Create(row, column)
            };
            swatch.Text = nameLookup[swatches[row, column].ToString()];
            swatch.TextChanged += Swatch_TextChanged;
            Grid.SetRow(swatch, row);
            Grid.SetColumn(swatch, column);
            return swatch;
        }

        private static readonly Dictionary<string, Color> colorLookup = typeof(Colors)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.Name != "Aqua" && p.Name != "Fuchsia")
            .ToDictionary(c => c.Name, c => (Color)c.GetValue(null), StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, string> nameLookup = colorLookup
            .ToDictionary(kvp => kvp.Value.ToString(), kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);
        private readonly Border sample;

        private static void Swatch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (TextBox)sender;
            var rowColumn = (Tuple<int, int>)textbox.Tag;
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Background = Brushes.White;
                swatches[rowColumn.Item1, rowColumn.Item2] = Colors.White;
                return;
            }

            Color c = Colors.Transparent;
            if (colorLookup.TryGetValue(textbox.Text, out c))
            {
                swatches[rowColumn.Item1, rowColumn.Item2] = c;
                textbox.Background = new SolidColorBrush(c);
                CopyColors();
            }
        }

        private static void CopyColors()
        {
            var sb = new StringBuilder();
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var dupes = new List<string>();

            for (int row = 0; row < swatches.GetLength(0); row++)
            {
                sb.Append("                { ");

                for (int column = 0; column < swatches.GetLength(1); column++)
                {
                    var color = swatches[row, column].ToString();
                    var name = nameLookup[color];
                    if (!set.Add(name))
                    {
                        dupes.Add(name);
                    }

                    sb.Append("Colors." + name);
                    if (column < swatches.GetLength(1) - 1)
                    {
                        sb.Append(", ");
                    }
                }

                sb.AppendLine(" },");
            }

            var missing = colorLookup.Keys.Distinct().Except(set, StringComparer.OrdinalIgnoreCase).OrderBy(s => s).ToArray();

            sb.AppendLine();

            sb.AppendLine("Dupes:");
            foreach (var dupe in dupes)
            {
                sb.AppendLine(dupe);
            }

            sb.AppendLine();
            sb.AppendLine("Missing:");
            foreach (var missingColor in missing)
            {
                sb.AppendLine(missingColor);
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}
