using System;
using System.IO;
using System.Linq;
using System.Numerics;
using ZenVis.Shared;

namespace ZenVis.Lib
{
    public class Visual
    {
        public string FileName;

        public string FileExtension;

        public int Index;

        public Vector3 Position;

        public zMAT3 Rotation;

        public Vector3 Scale;

        public bool Visible;

        public Visual(string fileName, Vector3 pos, zMAT3 rot3, bool visible)
        {
            try
            {
                fileName = fileName.ToUpper();

                if (!fileName.StartsWith("LEVEL"))
                {
                    string[] fileNameSplit = fileName.Split('.');

                    this.FileName = fileNameSplit[0];

                    if (fileNameSplit.Length == 2)
                    {
                        if (fileNameSplit[1].All(char.IsDigit))
                        {
                            this.FileExtension = "";
                            this.Index = int.Parse(fileNameSplit[1]);
                        }
                        else
                        {
                            this.FileExtension = fileNameSplit[1];
                            this.Index = 0;
                        }
                    }
                    else if (fileNameSplit.Length == 3)
                    {
                        this.FileExtension = fileNameSplit[1];
                        this.Index = int.Parse(fileNameSplit[2]);
                    }
                }
                else
                {
                    this.FileName = Path.GetFileNameWithoutExtension(fileName);
                    this.FileExtension = "3DS";
                    this.Index = 0;
                }
            }
            catch
            {
                throw new ArgumentException(Localization.Instance.GetTranslation("ObjectNameInvalid"), fileName);
            }
            this.Position = pos;
            this.Rotation = rot3;
            this.Visible = visible;
            string upper = Path.GetExtension(fileName);
            if (upper == ".ASC" || upper == ".MDS" || upper == ".MMS")
            {
                this.Scale = new Vector3(1f, -1f, -1f);
                return;
            }
            if (!(upper == ".3DS") && !(upper == ".MRM") && !(upper == ".MSH"))
            {
                this.Scale = new Vector3(1f, 1f, 1f);
                return;
            }
            this.Scale = new Vector3(-1f, -1f, 1f);
        }
    }
}