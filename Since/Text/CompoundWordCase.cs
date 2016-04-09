using System;
using System.Collections.Generic;
using System.Linq;

namespace Since.Text
{
    public enum WordCase
    {
        Any,
        LowerCase,
        TitleCase,
        UpperCase
    }

    public class CompoundWordCase
    {
        public WordCase Case { get; set; }
        public string Separator { get; set; }

        public bool HasSeparator
            => !string.IsNullOrEmpty(this.Separator);

        public static string ToCase(string str, WordCase wordCase)
        {
            if (string.IsNullOrEmpty(str) || wordCase == WordCase.Any)
                return str;

            switch (wordCase)
            {
                case WordCase.LowerCase:
                    return str.ToLower();
                case WordCase.TitleCase:
                    return char.ToUpper(str[0]) + str.Substring(1).ToLower();
                case WordCase.UpperCase:
                    return str.ToUpper();
                default:
                    return str;
            }
        }

        public string Join(IEnumerable<string> ss, string separator, WordCase wordCase)
            => string.Join(separator, ss.Select(s => s.ToCase(wordCase)));

        public static IEnumerable<string> Split(string compoundString, CompoundWordCase fromCase,
            WordCase toCase = WordCase.Any)
        {
            if (string.IsNullOrEmpty(compoundString))
                return Enumerable.Empty<string>();

            if (fromCase.HasSeparator)
            {
                return compoundString.Split(new[] {fromCase.Separator}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.ToCase(toCase));
            }
            if (fromCase.Case == WordCase.TitleCase)
            {
                return SplitOnTitleCase(compoundString)
                    .Select(s => s.ToCase(toCase));
            }
            return new[] {compoundString.ToCase(toCase)};
        }

        public static IEnumerable<string> SplitOnTitleCase(string compoundString)
        {
            var startIndex = 0;
            while (startIndex < compoundString.Length - 1)
            {
                var length = 1;
                while (char.IsLower(compoundString[startIndex + length]))
                    length++;
                yield return compoundString.Substring(startIndex, length);
                startIndex += length;
            }

            if (startIndex != compoundString.Length - 1)
                yield return compoundString.Substring(startIndex);
        }
    }

    internal static class Extensions
    {
        public static string ToCase(this string str, WordCase wordCase)
            => CompoundWordCase.ToCase(str, wordCase);
    }
}