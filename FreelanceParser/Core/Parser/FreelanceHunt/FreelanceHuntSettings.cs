using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceParser.Core.Freelances
{
    public class FreelanceHuntSettings : IParserSettings
    {
        public string BaseUrl { get; } = "https://freelancehunt.com/projects?skills%5B%5D=24";
        public string Prefix { get; } = "page={CurrentId}";
        public int StartPoint { get; } = 1;
        public int EndPoint { get; } = 1;
    }
}
