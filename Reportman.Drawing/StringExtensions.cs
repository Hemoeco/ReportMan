using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reportman.Drawing
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Split(
    this string text,
    char separator,
    char escapeCharacter, bool useextension, bool ex)
        {
            var builder = new StringBuilder(text.Length);

            bool escaped = false;
            foreach (var ch in text)
            {
                if (separator == ch && !escaped)
                {
                    yield return builder.ToString();
                    builder.Clear();
                }
                else
                {
                    // separator is removed, escape characters are kept
                    builder.Append(ch);
                }
                // set escaped for next cycle, 
                // or reset unless escape character is escaped.
                escaped = escapeCharacter == ch && !escaped;
            }
            yield return builder.ToString();
        }
        public static string[] Split(
            this string text,
            char separator,
            char escapeCharacter, bool escape)
        {
            List<string> nlist = new List<string>();
            foreach (string nstring in text.Split(separator, escapeCharacter, true, true))
            {
                nlist.Add(nstring);
            }
            return nlist.ToArray();
        }

        public static string Escape(this string text, string controlChars, char escapeCharacter)
        {
            var builder = new StringBuilder(text.Length + 3);
            foreach (var ch in text)
            {
                if (controlChars.Contains(ch))
                {
                    builder.Append(escapeCharacter);
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string Unescape(this string text, char escapeCharacter)
        {
            var builder = new StringBuilder(text.Length);
            bool escaped = false;
            foreach (var ch in text)
            {
                escaped = escapeCharacter == ch && !escaped;
                if (!escaped)
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
		        public static string RemoveDiacritics(this string input)
        {
            return StringUtil.RemoveDiacritics(input);
        }
        public static string QuoteStr(this string input)
        {
            return StringUtil.QuoteStr(input);
        }
        public static string DoubleQuoteStr(this string input)
        {
            return StringUtil.DoubleQuoteStr(input);
        }
    }
}
