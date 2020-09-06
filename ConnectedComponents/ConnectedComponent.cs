using System.Collections.Generic;

namespace ConnectedComponents
{
    /// <summary>
    /// A connected component is a set of horizontal segments ("spans")
    /// </summary>
    /// <remarks>
    /// It has three generations: 0, 1 and 2 (just like the Garbage Collector)
    /// Generation 0 means the current line of the matrix,
    /// Generation 1 means the line above it and
    /// Generation 2 means all the lines of the matrix above Generation 1
    /// </remarks>
    public class ConnectedComponent
    {
        public ConnectedComponent()
        {
            Spans = new SpanList();
            FrontalSpans = new SpanList();
            NewFrontalSpans = new SpanList();
        }

        /// <summary>
        /// Generation 0
        /// </summary>
        public SpanList NewFrontalSpans { get; set; }
        /// <summary>
        /// Generation 1
        /// </summary>
        public SpanList FrontalSpans { get; set; }
        /// <summary>
        /// Generation 2
        /// </summary>
        public SpanList Spans { get; set; }

        /// <summary>
        /// Just add all the spans from another component to the current one
        /// </summary>
        /// <param name="connectedComponent">The other component</param>
        public void UnionWith(ConnectedComponent connectedComponent)
        {
            Spans.UnionWith(connectedComponent.Spans);
            FrontalSpans.UnionWith(connectedComponent.FrontalSpans);
            NewFrontalSpans.UnionWith(connectedComponent.NewFrontalSpans);
        }

        public void ShiftGenerations(ConnectedComponentSet complete)
        {
            // add first generation to the second
            Spans.UnionWith(FrontalSpans);
            // if the Generation 0 hasn't been filled, 
            // it means the component is complete and won't grow anymore
            if (NewFrontalSpans.Count == 0)
            {
                // add it to the list of complete components
                complete.Add(this);
            }
            else
            {
                // Generation 0 grows older and becomes Generation 1
                FrontalSpans = NewFrontalSpans;
                // And a new Generation 0 is born
                NewFrontalSpans = new SpanList();
            }
        }
    }

    /// <summary>
    /// Just a set of components
    /// </summary>
    public class ConnectedComponentSet : List<ConnectedComponent>
    {
        /// <summary>
        /// Initializes a set of components with a single span of Generation 0
        /// </summary>
        /// <param name="span">The starting span ("tip of the eisberg")</param>
        public void AddNewComponentFromSpan(Span span)
        {
            ConnectedComponent newComponent = new ConnectedComponent();
            newComponent.NewFrontalSpans.Add(span);
            Add(newComponent);
        }
    }
}
