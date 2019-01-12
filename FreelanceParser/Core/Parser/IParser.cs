using AngleSharp.Html.Dom;
using System.Collections;
using System.Collections.Generic;

namespace FreelanceParser.Core
{
    public interface IParser<T> where T : class
    {
        IEnumerable<T> Parse(IHtmlDocument document);
    }
}
