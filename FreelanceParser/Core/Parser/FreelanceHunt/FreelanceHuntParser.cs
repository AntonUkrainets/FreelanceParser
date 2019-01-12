using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using FreelanceParser.Core.Model;
using FreelanceParser.Data;

namespace FreelanceParser.Core.Freelances
{
    public class FreelanceHuntParser : IParser<FreelanceHuntItem>
    {
        public IEnumerable<FreelanceHuntItem> Parse(IHtmlDocument document)
        {
            ICollection<FreelanceHuntItem> contents = new List<FreelanceHuntItem>();

            var tasks = document
                .QuerySelectorAll("tr")
                .Where(item => item.Attributes.Any(x => x.Name == "data-published"));

            foreach (var task in tasks)
            {
                var id = task.Attributes.FirstOrDefault(x => x.Name == "data-published")?.Value.Trim();
                var title = task.QuerySelector("tr td.left a")?.TextContent.Trim();
                var price = task.QuerySelector("tr td.text-center span div.text-green.price.with-tooltip")?.TextContent.Trim();
                var countBeats = task.QuerySelector("tr td.text-center.hidden-xs a.text-orange.price")?.TextContent.Trim();
                var date = task.QuerySelector("tr td.text-center.hidden-xs div.with-tooltip div.with-tooltip.calendar")?
                               .Attributes.FirstOrDefault(x => x.Name== "title")?.Value.Trim();
                var uri = task.QuerySelector("tr td.left a.bigger.visitable")?.Attributes.FirstOrDefault(x => x.Name == "href")?.Value.Trim();
                var fullUri = "https://freelancehunt.com" + uri;

                contents.Add(new FreelanceHuntItem
                {
                    Id = int.Parse(id),
                    Title = title,
                    Price = price,
                    CountBeats = countBeats,
                    Date = date,
                    Uri = fullUri
                });
            }

            return contents;
        }
    }
}
