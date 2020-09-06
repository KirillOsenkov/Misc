using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ConnectedComponents
{
    /// <summary>
    /// Paints a given field on a given control using given graphics
    /// </summary>
    public class FieldPainter
    {
        public static void Draw(Field field, Control canvas, Graphics graphics)
        {
            int width = canvas.ClientSize.Width;
            int heigth = canvas.ClientSize.Height;

            for (int i = 0; i < field.Size.Width; i++)
            {
                for (int j = 0; j < field.Size.Height; j++)
                {
                    float x = width * (float)i / field.Size.Width;
                    float y = heigth * (float)j / field.Size.Height;
                    using (Brush brush = new SolidBrush(field[i, j]))
                        graphics.FillRectangle(
                            brush,
                            x,
                            y,
                            width * (float)(i + 1) / field.Size.Width - x,
                            heigth * (float)(j + 1) / field.Size.Height - y
                            );
                }
            }
        }
    }
}
