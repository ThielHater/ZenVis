using System.Collections.Generic;

namespace ZenVis.Lib
{
    public class ImportSetting
    {
        public string Type;

        public Dictionary<string, string> Properties;

        public ImportSetting()
        {
            this.Type = "";
            this.Properties = new Dictionary<string, string>();
        }
    }
}