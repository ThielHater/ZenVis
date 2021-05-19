using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ZenVis.Shared
{
    public static class Helper
    {
        public static string ApplicationDirectory()
        {
            return Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath);
        }

        public static T DeserializeFromByteArray<T>(byte[] bytes)
        {
            return Helper.DeserializeFromString<T>(Encoding.UTF8.GetString(bytes));
        }

        public static T DeserializeFromString<T>(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }

        public static string EscapeArgument(string arg)
        {
            return string.Concat("\"", arg, "\"");
        }

        public static string FloatToString(float value)
        {
            return value.ToString("0.######", CultureInfo.InvariantCulture);
        }

        public static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
        {
            return filters.Split(new char[] { '|' }).SelectMany<string, string>((string filter) => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray<string>();
        }

        public static byte[] SerializeToByteArray(object obj)
        {
            return Encoding.UTF8.GetBytes(Helper.SerializeToString(obj));
        }

        public static string SerializeToString(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static float StringToFloat(string value)
        {
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] tArray = new T[length];
            Array.Copy(data, index, tArray, 0, length);
            return tArray;
        }

        public static void CopyDirectory(string source, string dest, SearchOption searchOption)
        {
            if (searchOption == SearchOption.AllDirectories)
                foreach (string dir in Directory.GetDirectories(source, "*", searchOption))
                    Directory.CreateDirectory(dir.Replace(source, dest));

            foreach (string file in Directory.GetFiles(source, "*", searchOption))
                File.Copy(file, file.Replace(source, dest), true);
        }

        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, UInt32 uFlags);

        public static void ResetWindow(string title)
        {
            IntPtr hWnd = FindWindow(IntPtr.Zero, title);

            if (hWnd != IntPtr.Zero)
                ResetWindow(hWnd);
        }

        public static void ResetWindow(IntPtr hWnd, int width = 0, int height = 0)
        {
            SetWindowPos(hWnd, new IntPtr(-2), 0, 0, width, height, SWP_SHOWWINDOW | ((width == 0) && (height == 0) ? SWP_NOSIZE : 0));
        }
    }
}