using System.Text.RegularExpressions;

namespace ZenVis.Lib
{
    internal class Parser
    {
        public static Regex RegVisualType;

        public static Regex RegVisualRotation;

        public static Regex RegVisualPosition;

        public static Regex RegVisualFileName;

        public static Regex RegVisualVisible;

        public static Regex RegVisualItemInstance;

        public static Regex RegVisualDimensions;

        public static Regex RegItemPrototype;

        public static Regex RegItemInstance;

        public static Regex RegItemVisual;

        public static Regex RegProperty;

        public static Regex RegHexadecimal;

        static Parser()
        {
            Parser.RegVisualType = new Regex("(\\s*)\\[%(\\s)([a-zA-Z:]+)(\\s)([0-9]+)(\\s)([0-9]+)\\]", RegexOptions.IgnoreCase);
            Parser.RegVisualRotation = new Regex("(\\s*)trafoOSToWSRot\\=raw\\:([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);
            Parser.RegVisualPosition = new Regex("(\\s*)trafoOSToWSPos\\=vec3\\:([0-9-_.]+)\\s([0-9-_.]+)\\s([0-9-_.]+)", RegexOptions.IgnoreCase);
            Parser.RegVisualFileName = new Regex("(\\s*)visual\\=string\\:(.*)", RegexOptions.IgnoreCase);
            Parser.RegVisualVisible = new Regex("(\\s*)showVisual\\=bool\\:([0-1])", RegexOptions.IgnoreCase);
            Parser.RegVisualItemInstance = new Regex("(\\s*)itemInstance\\=string\\:(.*)", RegexOptions.IgnoreCase);
            Parser.RegVisualDimensions = new Regex("(\\s*)decalDim\\=rawFloat\\:([0-9-_.]+)\\s([0-9-_.]+)\\s", RegexOptions.IgnoreCase);
            Parser.RegItemPrototype = new Regex("(\\s*)Prototype(\\s+)([a-zA-Z0-9_]+)(\\s*)\\(C_Item\\)", RegexOptions.IgnoreCase);
            Parser.RegItemInstance = new Regex("(\\s*)Instance(\\s+)([a-zA-Z0-9_]+)(\\s*)\\(([a-zA-Z0-9_]+)\\)", RegexOptions.IgnoreCase);
            Parser.RegItemVisual = new Regex("(\\s*)visual(\\s*)=(\\s*)\"(.*)\"(\\s*);", RegexOptions.IgnoreCase);
            Parser.RegProperty = new Regex("(\\s*)(.*)=(.*):(.*)");
            Parser.RegHexadecimal = new Regex("[0-9a-fA-F]+");
        }

        public Parser()
        {
        }
    }
}