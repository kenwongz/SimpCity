namespace SimpCity {
    class Utils {
        public static string RepeatString(string s, int n) {
            string ret = "";
            for (int i = 0; i < n; i++) {
                ret += s;
            }
            return ret;
        }
    }
}
