using System;
using System.Collections.Generic;
using System.Text;

namespace HubCloud.ParserCollection
{
    public class SeparateNameValuesJsonParserSettings
    {
        public string NamesPath { get; set; }
        public string ValuesPath { get; set; }
        public bool IgnoreDoubleNames { get; set; } = true;
        public Dictionary<string, string> NamesMapping { get; set; } = new Dictionary<string, string>();
    }
}
