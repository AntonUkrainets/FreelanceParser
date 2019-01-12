using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreelanceParser.Core
{
    public interface IParserSettings
    {
        string BaseUrl { get; }
        string Prefix { get; }
        int StartPoint { get; }
        int EndPoint { get; }
    }
}
