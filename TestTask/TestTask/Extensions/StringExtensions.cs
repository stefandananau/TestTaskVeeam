using System.IO;

namespace TestTask.Extensions
{
    public static class StringExtensions
    {
        public static bool IsPath(this string s)
        {
            try
            {
                return Path.IsPathRooted(s);
            }
            catch 
            { 
                return false; 
            }
        }

        public static string AddTxtExtension(this string s)
        {
            if (s.EndsWith(".txt")) return s;
            return s + ".txt";
        }
    }
}
