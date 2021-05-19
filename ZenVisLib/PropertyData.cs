namespace ZenVis.Lib
{
    public class PropertyData
    {
        private string name;

        private string type;

        private string data;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public string Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        public PropertyData(string name, string type, string data)
        {
            this.Name = name;
            this.Type = type;
            this.Data = data;
        }
    }
}