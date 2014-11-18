using System;
using System.IO;
using System.Text;

namespace DropNet.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string value)
        {
            value = Uri.EscapeDataString(value);

            StringBuilder builder = new StringBuilder();
            foreach (char ch in value)
            {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~%".IndexOf(ch) != -1)
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append('%' + string.Format("{0:X2}", (int)ch));
                }
            }
            return builder.ToString();
        }

        public static string AppendDirectory(this string directory, string subDirectory)
        {
            if (directory.EndsWith("/"))
                return directory + subDirectory;

            return directory + "/" + subDirectory;
        }

        public static string RemoveLastDirectory(this string directory)
        {
            var index = directory.LastIndexOf('/');
            if (index <= 0)
                return "/";
            return directory.Substring(0, index);
        }

        public static string GetRandomString(int length)
        {
            var sb = new StringBuilder(length);
            var rnd = new Random(Environment.TickCount);
            for (int i = 0; i < length; i++)
                sb.Append((char)rnd.Next(char.MaxValue));
            return sb.ToString();
        }

    }
}
