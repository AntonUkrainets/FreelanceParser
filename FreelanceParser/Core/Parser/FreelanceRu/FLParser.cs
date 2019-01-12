using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using FreelanceParser.Core.Model;
using FreelanceParser.Data;

namespace FreelanceParser.Core.Freelances
{
    public class FLParser : IParser<FlRuItem>
    {
        public IEnumerable<FlRuItem> Parse(IHtmlDocument document)
        {
            ICollection<FlRuItem> contents = new List<FlRuItem>();

            var tasksNode = document
                    .QuerySelectorAll("div")
                    .FirstOrDefault(item => item.ClassName == "projects");

            var tasks = tasksNode
                .QuerySelectorAll("div")
                .Where(x => x.ClassName.StartsWith("proj "));

            foreach (var task in tasks)
            {
                var id = task.Attributes.FirstOrDefault(x => x.Name == "data-project-id")?.Value.Trim();
                var title = task.QuerySelector("div.p_title h2 a.ptitle span")?.TextContent.Trim();
                var price = task.QuerySelector("div.p_title h2 span.cost a.cost_val")?.TextContent.Trim();
                var countBeats = task.QuerySelector("ul.list-inline li.proj-inf.messages.pull-left a")?.TextContent.Trim();
                var date = task.QuerySelector("ul.list-inline li.proj-inf.pdata.pull-left")?.TextContent;
                var uri = task.QuerySelector("div.p_title h2 a.ptitle")?.Attributes.FirstOrDefault(item => item.Name == "href")?.Value.Trim();
                var fullUri = new FLSettings().Url + uri;

                contents.Add(new FlRuItem
                {
                    Id = int.Parse(id),
                    Title = title,
                    Price = price,
                    CountBeats = countBeats,
                    Date = date,
                    Uri = fullUri,
                });
            }

            return contents;
        }
    }
}
