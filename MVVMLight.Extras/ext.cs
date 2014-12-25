using System;

namespace MVVMLight.Extras
{
    public static class ext
    {
        public static string RemoveEnd(this string s, string end, StringComparison c = StringComparison.CurrentCultureIgnoreCase)
        {
            var len = end.Length;
            if (s.EndsWith(end, c))
            {
                return s.Substring(0, s.Length - len);
            }
            else
            {
                throw new WrongStringEndException();
            }
        }
        public static string RemoveEndIfExists(this string s, string end, StringComparison c = StringComparison.CurrentCultureIgnoreCase)
        {
            var len = end.Length;
            if (s.EndsWith(end, c))
            {
                return s.Substring(0, s.Length - len);
            }
            else
            {
                return s;
            }
        }
    }
}