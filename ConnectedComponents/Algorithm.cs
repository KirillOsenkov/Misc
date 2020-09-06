using System.Drawing;
using System.Linq;

namespace ConnectedComponents
{
    /// <summary>
    /// This is the main algorithm.
    /// </summary>
    public static class Algorithm
    {
        /// <summary>
        /// Calculates and returns a set of 4-connected components in a field
        /// </summary>
        /// <param name="field">The two-dimensional matrix with colors</param>
        /// <returns></returns>
        public static ConnectedComponentSet FindConnectedComponents(Field field)
        {
            ConnectedComponentSet result = new ConnectedComponentSet();
            ConnectedComponentSet current = new ConnectedComponentSet();

            // we scan the entire matrix line by line
            for (int y = 0; y < field.Size.Height; y++)
            {
                SpanList spansOnCurrentLine = GetSpansFromLine(field, y);
                foreach (var span in spansOnCurrentLine)
                {
                    PlaceSpan(span, current);
                }

                // check which components didn't change since the previous iteration
                ConnectedComponentSet complete = new ConnectedComponentSet();

                foreach (var component in current)
                {
                    component.ShiftGenerations(complete);
                }

                foreach (var component in complete)
                {
                    component.FrontalSpans.Clear();
                    current.Remove(component);
                    result.Add(component);
                }
            }

            // We reached the bottom
            // For all the remaining components, add them to the result
            foreach (var component in current)
            {
                component.Spans.UnionWith(component.FrontalSpans);
                component.Spans.UnionWith(component.NewFrontalSpans);
                result.Add(component);
            }

            return result;
        }

        /// <summary>
        /// Retrieves a sequence of adjacent horizontal same-color segments
        /// </summary>
        /// <param name="field">from this field</param>
        /// <param name="y"></param>
        /// <returns></returns>
        static SpanList GetSpansFromLine(Field field, int y)
        {
            SpanList result = new SpanList();
            Color currentElement = field[0, y];
            Span current = new Span() { StartX = 0, Y = y, Color = currentElement };

            for (int x = 1; x < field.Size.Width; x++)
            {
                if (field[x, y] != currentElement)
                {
                    currentElement = field[x, y];
                    current.EndX = x - 1;
                    result.Add(current);
                    current = new Span() { StartX = x, Y = y, Color = currentElement };
                }
            }

            current.EndX = field.Size.Width - 1;
            result.Add(current);
            return result;
        }

        /// <summary>
        /// Classifies a span with regard to already processed spans
        /// </summary>
        /// <param name="currentSpan"></param>
        /// <param name="currentComponentSet"></param>
        static void PlaceSpan(Span currentSpan, ConnectedComponentSet currentComponentSet)
        {
            if (currentSpan.Y == 0)
            {
                // for the first row, just accumulate the spans in the Generation 0
                currentComponentSet.AddNewComponentFromSpan(currentSpan);
                return;
            }

            // find all spans from Generation 1 that "touch" this span
            ConnectedComponentSet componentsToJoin = new ConnectedComponentSet();
            componentsToJoin.AddRange(
                currentComponentSet.Where(
                    component => component.FrontalSpans.IntersectsWith(currentSpan)));

            // if we "touch" 0 existing components, create a new one
            if (componentsToJoin.Count == 0)
            {
                currentComponentSet.AddNewComponentFromSpan(currentSpan);
            }
            // if we touch one component, append the current span to its bottom
            else if (componentsToJoin.Count == 1)
            {
                componentsToJoin[0].NewFrontalSpans.Add(currentSpan);
            }
            // if we touch more than one component, it means they all belong together.
            // Now we unite all of these components into one.
            else
            {
                for (int i = 1; i < componentsToJoin.Count; i++)
                {
                    componentsToJoin[0].NewFrontalSpans.Add(currentSpan);
                    componentsToJoin[0].UnionWith(componentsToJoin[i]);
                    currentComponentSet.Remove(componentsToJoin[i]);
                }
            }
        }
    }
}
