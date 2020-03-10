using System.Collections.Generic;

namespace XpathDataCrawler.DataStructure.Model
{
    public class Method
    {
        public readonly List<string> URL = new List<string>();
        public readonly List<List<string>> Keys = new List<List<string>>();
        public readonly List<List<string>> Values = new List<List<string>>();
    }
}
