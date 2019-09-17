using System.Collections.Generic;

namespace com.MovieAssistant.core.DataStructure
{
    internal class SubDataPackage
    {
        public SubData[] SubDatas { get; set; }
        public List<SubDataPackage> NextSubDataPackage { get; set; } = new List<SubDataPackage>();
    }
}
