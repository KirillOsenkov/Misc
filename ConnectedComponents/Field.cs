using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConnectedComponents
{
    /// <summary>
    /// A two-dimensional matrix with cells of type Color
    /// </summary>
    public class Field : FieldBase<Color>
    {
        public Field() : this(0.5, 5)
        {
        }

        public Field(double whitePercentage, int fieldSize) : base(new Size(fieldSize, fieldSize))
        {
            WhitePercentage = whitePercentage;
            InitializeField();
        }

        public double WhitePercentage { get; set; }

        void InitializeField()
        {
            field = new Color[Size.Width, Size.Height];
            for (int i = 0; i < Size.Width; i++)
            {
                for (int j = 0; j < Size.Height; j++)
                {
                    this[i, j] = GetRandomElement();
                }
            }
        }

        static Random random = new Random();

        Color GetRandomElement()
        {
            return random.Next(100) < 100 * WhitePercentage ? Color.White : Color.Black;
        }

        public static Color GetRandomColor()
        {
            return Color.FromArgb(
                random.Next(0, 256),
                random.Next(0, 256),
                random.Next(0, 256));
        }
    }
}
