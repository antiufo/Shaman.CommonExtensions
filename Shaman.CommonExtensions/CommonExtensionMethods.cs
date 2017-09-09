using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shaman
{
    public static class CommonExtensionMethods
    {


        public static IEnumerable<T> RecursiveEnumeration<T>(this T first, Func<T, T> parent)
        {
            var current = first;

            while (current != null)
            {
                yield return current;
                current = parent(current);

            }

        }

        public static string TrimEnd(this string str, string text)
        {
            if (str == null) throw new ArgumentNullException();
            if (str.EndsWith(text)) return str.Substring(0, str.Length - text.Length);
            else return str;
        }


        public static string TrimStart(this string str, string text)
        {
            if (str == null) throw new ArgumentNullException();
            if (str.StartsWith(text)) return str.Substring(text.Length);
            else return str;
        }


        public static string RightSubstring(this string str, int size)
        {
            return str.Substring(str.Length - size, size);
        }

#if !SALTARELLE
        public static bool Contains(this string str, string substring, StringComparison comparison)
        {
            return str.IndexOf(substring, comparison) != -1;
        }
#endif




        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : class
        {

            TValue value;
            if (dict.TryGetValue(key, out value)) return value;
            else return null;
        }

#if !SALTARELLE
        public static TValue? TryGetNullableValue<TKey, TValue>(
#if NET35
            this IDictionary<TKey, TValue> dict,
#else
            this IReadOnlyDictionary<TKey, TValue> dict,
#endif
            TKey key) where TValue : struct
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
#endif

        public static string TryCapture(this string str, string pattern)
        {
            if (str == null) throw new ArgumentNullException();

#if SALTARELLE
            var match = new System.Text.RegularExpressions.Regex(pattern).Exec(str);
            if (match == null) return null;
            return match[1];
#else
            var match = System.Text.RegularExpressions.Regex.Match(str, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);
            if (!match.Success) return null;
            return match.Groups[1].Value;
#endif
        }



        public static string Capture(this string str, string pattern)
        {
            if (str == null) throw new ArgumentNullException();
#if SALTARELLE
            var match = new System.Text.RegularExpressions.Regex(pattern).Exec(str);
            if (match == null)
            {
                throw new ArgumentException("Cannot find the specified regex pattern.");
            }
            return match[1];
#else
            var match = System.Text.RegularExpressions.Regex.Match(str, pattern, System.Text.RegularExpressions.RegexOptions.Singleline);
            if (!match.Success)
            {
                var ex = new InvalidDataException();
#if !NETFX_CORE
                ex.Data.Add("SourceString", str);
                ex.Data.Add("Pattern", pattern);
#endif
                throw ex;
            }
            return match.Groups[1].Value;
#endif
        }

        public static string RegexReplace(this string str, string pattern, string replacement)
        {
            if (str == null) throw new ArgumentNullException();
#if SALTARELLE
            return str.Replace(new System.Text.RegularExpressions.Regex(pattern, "g"), replacement);
#else
            return System.Text.RegularExpressions.Regex.Replace(str, pattern, replacement, System.Text.RegularExpressions.RegexOptions.Singleline);
#endif
        }


#if !SALTARELLE
        public static string RegexReplace(this string str, System.Text.RegularExpressions.Regex pattern, string replacement)
        {
            if (str == null) throw new ArgumentNullException();

            return pattern.Replace(str, replacement);
        }
        public static bool Contains(this string str, System.Text.RegularExpressions.Regex pattern)
        {
            if (str == null) throw new ArgumentNullException();

            return pattern.IsMatch(str);
        }
#endif


        /// <summary>
        /// Provides an alternate syntax for IEnumerable.Contains()
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="obj">The item to find.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>Whether the item is contained in the collection or not.</returns>
        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            if (obj == null) throw new ArgumentNullException();
            return collection.Contains(obj);
        }


        /// <summary>
        /// Provides an alternate syntax for IEnumerable.Contains()
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="obj">The item to find.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>Whether the item is contained in the collection or not.</returns>
        public static bool In<T>(this T obj, params T[] collection)
        {
            if (obj == null) throw new ArgumentNullException();
            return collection.Contains(obj);
        }
        
        public static bool In<T>(this T obj, T option1)
        {
            var comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(obj, option1)) return true;
            return false;
        }
        
        public static bool In<T>(this T obj, T option1, T option2)
        {
            var comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(obj, option1)) return true;
            if (comparer.Equals(obj, option2)) return true;
            return false;
        }
        
        public static bool In<T>(this T obj, T option1, T option2, T option3)
        {
            var comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(obj, option1)) return true;
            if (comparer.Equals(obj, option2)) return true;
            if (comparer.Equals(obj, option3)) return true;
            return false;
        }
        
        public static bool In<T>(this T obj, T option1, T option2, T option3, T option4)
        {
            var comparer = EqualityComparer<T>.Default;
            if (comparer.Equals(obj, option1)) return true;
            if (comparer.Equals(obj, option2)) return true;
            if (comparer.Equals(obj, option3)) return true;
            if (comparer.Equals(obj, option4)) return true;
            return false;
        }


        private static string CaptureBetween(this string text, string begin, string end, bool optional)
        {
            var beginPos = text.IndexOf(begin);
            var endSearchStart = beginPos + begin.Length;
            var endPos = beginPos == -1 || endSearchStart > text.Length ? -1 : text.IndexOf(end, endSearchStart);
            if (beginPos == -1 || endPos == -1)
            {
                if (optional) return null;
#if SALTARELLE
                var ex = new Exception();
#else
                var ex = new InvalidDataException();
                ex.Data.Add("SourceData", text);
                ex.Data.Add("BeginString", begin);
                ex.Data.Add("EndString", end);
#endif
                throw ex;
            }
            beginPos += begin.Length;
            return text.Substring(beginPos, endPos - beginPos);
        }

        public static string CaptureBetween(this string text, string begin, string end)
        {
            return CaptureBetween(text, begin, end, false);
        }

        public static string TryCaptureBetween(this string text, string begin, string end)
        {
            return CaptureBetween(text, begin, end, true);
        }


#if !SALTARELLE
        public static string CaptureAfter(this string text, string prefix)
        {
            var idx = text.IndexOf(prefix);
            if (idx == -1) throw new InvalidDataException();
            return text.Substring(idx + prefix.Length);
        }
        public static string TryCaptureAfter(this string text, string prefix)
        {
            var idx = text.IndexOf(prefix);
            if (idx == -1) return null;
            return text.Substring(idx + prefix.Length);
        }
        public static string CaptureBefore(this string text, string suffix)
        {
            var idx = text.IndexOf(suffix);
            if (idx == -1) throw new InvalidDataException();
            return text.Substring(0, idx);
        }
        public static string TryCaptureBefore(this string text, string suffix)
        {
            var idx = text.IndexOf(suffix);
            if (idx == -1) return null;
            return text.Substring(0, idx);
        }
#endif



    }
}

