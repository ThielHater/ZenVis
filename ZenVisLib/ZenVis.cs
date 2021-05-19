using System.IO;
using ZenVis.Shared;

namespace ZenVis.Lib
{
    public class ZenVis
    {
        public string AppDir;

        public string GothicDir;

        public string GothicVersion;

        public string BlenderExe;

        public string OutputDir;

        public ZenVis(string appDir, string gothicDir, string gothicVersion, string blenderExe, string outputDir)
        {
            this.AppDir = string.Concat(appDir, "\\");
            this.GothicDir = string.Concat(gothicDir, "\\");
            this.GothicVersion = gothicVersion;
            this.BlenderExe = blenderExe;
            this.OutputDir = string.Concat(outputDir, "\\");
            if (!Directory.Exists(string.Concat(this.AppDir, "Blender")))
            {
                throw new DirectoryNotFoundException();
            }
            Directory.CreateDirectory(this.OutputDir);
        }
    }
}