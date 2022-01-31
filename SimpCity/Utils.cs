using System;
using System.Diagnostics.CodeAnalysis;

namespace SimpCity {
    public static class Utils {
        public static string RepeatString(string s, int n) {
            string ret = "";
            for (int i = 0; i < n; i++) {
                ret += s;
            }
            return ret;
        }

        [ExcludeFromCodeCoverage]
        public static void WriteLineColored(string value, ConsoleColor? foreground = null, ConsoleColor? background = null) {
            if (foreground != null) Console.ForegroundColor = foreground.Value;
            if (background != null) Console.BackgroundColor = background.Value;
            Console.Write(value);
            Console.ResetColor();
            // Do not allow buffers after content to be colored, including newlines
            Console.WriteLine();
        }
    }
}
