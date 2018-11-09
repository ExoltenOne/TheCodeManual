using System;
using System.Text;

namespace StringReplacer
{
    using System.Collections.Generic;

    public static class StringUtils
    {
        [ThreadStatic]
        private static StringBuilder m_ReplaceSB;

        private static StringBuilder GetReplaceSB(int capacity)
        {
            var result = m_ReplaceSB;

            if (null == result)
            {
                result = new StringBuilder(capacity);
                m_ReplaceSB = result;
            }
            else
            {
                result.Clear();
                result.EnsureCapacity(capacity);
            }

            return result;
        }

        public static string ReplaceAny(this string s, char replaceWith, params char[] chars)
        {
            if (null == chars)
                return s;

            if (null == s)
                return null;

            StringBuilder sb = null;

            for (int i = 0, count = s.Length; i < count; i++)
            {
                var temp = s[i];
                var replace = false;

                for (int j = 0, cc = chars.Length; j < cc; j++)
                    if (temp == chars[j])
                    {
                        if (null == sb)
                        {
                            sb = GetReplaceSB(count);
                            if (i > 0)
                                sb.Append(s, 0, i);
                        }

                        replace = true;
                        break;
                    }

                if (replace)
                    sb.Append(replaceWith);
                else
                if (null != sb)
                    sb.Append(temp);
            }

            return null == sb ? s : sb.ToString();
        }

        public static string MultiReplace(this string s, char[] toReplace, char replacement)
        {
            var builder = new StringBuilder(s);

            HashSet<char> set = new HashSet<char>(toReplace);
            for (int i = 0; i < builder.Length; ++i)
            {
                var currentCharacter = builder[i];
                if (set.Contains(currentCharacter))
                {
                    builder[i] = replacement;
                }
            }

            return builder.ToString();
        }

        public static string ReplaceAll(this string original, string toBeReplaced, string newValue)
        {
            if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(toBeReplaced)) return original;
            if (newValue == null) newValue = string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (char ch in original)
            {
                if (toBeReplaced.IndexOf(ch) < 0) sb.Append(ch);
                else sb.Append(newValue);
            }
            return sb.ToString();
        }

        public static string ReplaceAll(this string original, string[] toBeReplaced, string newValue)
        {
            if (string.IsNullOrEmpty(original) || toBeReplaced == null || toBeReplaced.Length <= 0) return original;
            if (newValue == null) newValue = string.Empty;
            foreach (string str in toBeReplaced)
                if (!string.IsNullOrEmpty(str))
                    original = original.Replace(str, newValue);
            return original;
        }
    }
}
