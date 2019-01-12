using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceParser.Core.Freelances
{
    public class FLSettings : IParserSettings
    {
        public string BaseUrl { get; } = "https://freelance.ru/projects/?cat=4&spec=108";
        public string Url { get; } = "https://freelance.ru/";
        public string Prefix { get; } = "page={CurrentId}";
        public int StartPoint { get; } = 1;
        public int EndPoint { get; } = 1;
    }
}
