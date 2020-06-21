using System;using System.Linq;

namespace BottlenoseGradeChecker
{
    /// <summary>
    /// Extension methods for existing types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Performs the rot-13 cipher on a string.
        /// </summary>
        /// <param name="input">The string to cipher.</param>
        /// <returns>The original string with each alphabetic character rotated by 13 positions.</returns>
        public static string Rot13(this string input)
        {
            return new string(input.ToCharArray().Select(s => { return (char)((s >= 97 && s <= 122) ? ((s + 13 > 122) ? s - 13 : s + 13) : (s >= 65 && s <= 90 ? (s + 13 > 90 ? s - 13 : s + 13) : s)); }).ToArray());
        }
    }
}
