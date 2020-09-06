using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConnectedComponents
{
    /// <summary>
    /// A maybe reusable base for two-dimensional matrices
    /// May be used for various games
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class FieldBase<T>
    {
        public FieldBase(Size initialSize)
        {
            Size = initialSize;
        }

        protected T[,] field;

        /// <summary>
        /// Let's hide the two-dimensional array from the clients
        /// </summary>
        public T this[int x, int y]
        {
            get
            {
                return field[x, y];
            }
            set
            {
                field[x, y] = value;
            }
        }

        private Size mSize;
        public Size Size
        {
            get
            {
                return mSize;
            }
            set
            {
                if (value.Width <= 0 || value.Height <= 0)
                {
                    return;
                }
                mSize = value;
            }
        }
    }
}
