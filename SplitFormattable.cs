    [Theory]
    [InlineData("{a}", "{a}")]
    [InlineData("{a}{b}", "{a}", "{b}")]
    [InlineData("{a}c", "{a}", "c")]
    [InlineData("c{a}", "c", "{a}")]
    [InlineData("c{a}b", "c", "{a}", "b")]
    [InlineData("{a}b{c}", "{a}", "b", "{c}")]
    [InlineData("{", "{")]
    [InlineData("}", "}")]
    [InlineData("{}", "{}")]
    [InlineData("}{", "}", "{")]
    [InlineData("}{}", "}", "{}")]
    [InlineData("{}{", "{}", "{")]
    [InlineData("{} {", "{}", " ", "{")]
    public void SplitFormat(params string[] inputs)
    {
        var input = inputs[0];
        var split = SplitFormattable(input);
        var chunks = split.Select(span => input.Substring(span.Start, span.Length)).ToArray();
        Assert.Equal(inputs.Skip(1).ToArray(), chunks);
    }

    /// <summary>
    /// Splits a given string into chunks between { and } and in-between text.
    /// E.g. for "{a}b{c}" it returns "{a}", "b" and "{c}".
    /// </summary>
    /// <returns>A list of chunks that when concatenated results in the initial string.</returns>
    public static IReadOnlyList<Span> SplitFormattable(string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return Array.Empty<Span>();
        }

        var result = new List<Span>();

        int start = 0;

        for (int i = 0; i < format.Length; i++)
        {
            char c = format[i];
            char next = (char)0;
            if (i < format.Length - 1)
            {
                next = format[i + 1];
            }

            switch (c)
            {
                case '{':
                    if (next != '{')
                    {
                        if (start < i)
                        {
                            result.Add(new Span(start, i - start));
                        }

                        start = i;
                    }

                    break;
                case '}':
                    if (next != '}')
                    {
                        result.Add(new Span(start, i - start + 1));
                        start = i + 1;
                    }

                    break;
                default:
                    break;
            }
        }

        if (start <= format.Length - 1)
        {
            result.Add(new Span(start, format.Length - start));
        }

        return result;
    }
