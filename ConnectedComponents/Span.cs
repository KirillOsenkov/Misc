using System.Collections.Generic;
using System.Drawing;

namespace ConnectedComponents
{
    /// <summary>
    /// A Span is a horizontal row of adjacent pixels of the same color
    /// </summary>
    public struct Span
    {
        /// <summary>
        /// Start coordinate of the pixel row
        /// </summary>
        public int StartX;
        /// <summary>
        /// End coordinate of the pixel row
        /// </summary>
        public int EndX;
        /// <summary>
        /// The vertical coordinate of the row
        /// </summary>
        public int Y;
        /// <summary>
        /// All pixels in this row are of the same color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Determines whether two rows intersect horizontally
        /// </summary>
        /// <param name="relativeTo">The row right above (or below) current</param>
        /// <returns></returns>
        public bool IntersectsWith(Span relativeTo)
        {
            return Color == relativeTo.Color 
                && !(StartX > relativeTo.EndX || EndX < relativeTo.StartX);
        }
    }

    public class SpanList : List<Span>
    {
        public void UnionWith(SpanList otherSpans)
        {
            this.AddRange(otherSpans);
        }

        public bool IntersectsWith(Span relativeTo)
        {
            foreach (var span in this)
            {
                if (span.IntersectsWith(relativeTo))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
