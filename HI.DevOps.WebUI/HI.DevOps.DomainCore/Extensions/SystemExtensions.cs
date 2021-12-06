using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HI.DevOps.DomainCore.Extensions
{
    /// <summary>
    ///     Contains the extension methods for framework classes.
    /// </summary>
    public static class SystemExtensions
    {
        public static StringBuilder RemoveLastWord(this StringBuilder sb, string value)
        {
            if (!sb.ToString().TrimEnd().Substring(sb.ToString().TrimEnd().Length - value.Length)
                .Contains(value)) return sb;
            if (sb.Length < 1) return sb;
            sb.Remove(sb.ToString().ToLower().LastIndexOf(value, StringComparison.Ordinal), value.Length);
            return sb;
        }
        public static string RemoveLastWords(this string input, int numberOfLastWordsToBeRemoved, char delimitter)
        {
            string[] words = input.Split(new[] { delimitter });
            words = words.Reverse().ToArray();
            words = words.Skip(numberOfLastWordsToBeRemoved).ToArray();
            words = words.Reverse().ToArray();
            return string.Join(delimitter.ToString(), words);
           
        }
        public static (DateTime, DateTime) GetWeekStartEndDate(this string iDate)
        {
            var baseDate = iDate.IsNullOrEmpty() ? DateTime.Today : Convert.ToDateTime(iDate);
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            return (thisWeekStart, thisWeekEnd);
        }
        /// <summary>
        ///     Indicates whether the specified value is null
        ///     (Nothing in Visual Basic) or actually empty after being trimmed
        ///     off of trailing blank spaces.
        /// </summary>
        /// <param name="value">
        ///     A <see cref="System.String" /> reference that has to be checked for
        ///     null.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" /> value indicating whether the
        ///     specified value is null/empty or not.
        /// </returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        /// <summary>
        ///     Indicates whether the specified value is null
        ///     (Nothing in Visual Basic) or does not have any items.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the objects that are contained by the value.
        /// </typeparam>
        /// <param name="value">
        ///     A <see cref="System.Collections.Generic.IEnumerable{T}" /> reference that
        ///     has to be checked for null.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" /> value indicating whether the
        ///     specified value is null/empty or not.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || value.Count() == 0;
        }

        /// <summary>
        ///     Indicates whether the specified value is null
        ///     (Nothing in Visual Basic) or does not have any items.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the objects that represent the key in the dictionary.
        /// </typeparam>
        /// <typeparam name="TK">
        ///     The type of the objects that represents the value in the dictionary.
        /// </typeparam>
        /// <param name="value">
        ///     A <see cref="System.Collections.Generic.IDictionary{T, K}" />
        ///     reference that has to be checked for null.
        /// </param>
        /// <returns>
        ///     A <see cref="System.Boolean" /> value indicating whether the
        ///     specified value is null/empty or not.
        /// </returns>
        public static bool IsNullOrEmpty<T, TK>(this IDictionary<T, TK> value)
        {
            return value == null || value.Count == 0;
        }

        /// <summary>
        ///     Trims trailing spaces in the specified value without failing on
        ///     nulls.
        /// </summary>
        /// <param name="value">
        ///     A <see cref="System.String" /> reference that has to be trimmed.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String" /> that contains the trimmed vlaue.
        /// </returns>
        public static string SafeTrim(this string value)
        {
            if (value == null || value.Trim().Length == 0)
                return null;

            return value.Trim();
        }

        /// <summary>
        ///     Get Description of the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is Enum)
            {
                var type = e.GetType();
                var values = Enum.GetValues(type);

                foreach (int val in values)
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val) ?? throw new InvalidOperationException());
                        var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (descriptionAttributes.Length > 0)
                            // we're only getting the first description we find
                            // others will be ignored
                            description = ((DescriptionAttribute) descriptionAttributes[0]).Description;

                        break;
                    }
            }

            return description;
        }

        /// <summary>
        ///     Get the Integer value of the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int ToInt<T>(this T e) where T : IConvertible
        {
            var value = 0;

            if (!(e is Enum)) return value;
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val != e.ToInt32(CultureInfo.InvariantCulture)) continue;
                value = val;

                break;
            }

            return value;
        }

        public static TDerived ToDerived<TBase, TDerived>(TBase tBase)
            where TDerived : TBase, new()
        {
            var tDerived = new TDerived();
            foreach (var propBase in typeof(TBase).GetProperties())
            {
                var propDerived = typeof(TDerived).GetProperty(propBase.Name);
                if (propDerived != null && propDerived.CanWrite)
                    propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
            }

            return tDerived;
        }

        public static int ToInt(this string number)
        {
            return int.TryParse(number, out var parsedInt) ? parsedInt : 0;
        }

        public static int HexToInt(this string number)
        {
            if (string.IsNullOrEmpty(number)) return -1;
            return Convert.ToInt32(number, 16);
        }

        public static decimal ToDecimal(this string number)
        {
            return decimal.TryParse(number, out var parsed) ? parsed : 0;
        }

        public static float ToFloat(this string number)
        {
            return float.TryParse(number, out var parsed) ? parsed : 0;
        }

        public static float ToFloat(this decimal number)
        {
            return (float) number;
        }

        public static object ToDbNull(this object param)
        {
            return param ?? DBNull.Value;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static bool IsAnyNullOrEmpty(this object myObject)
        {
            foreach (var pi in myObject.GetType().GetProperties())
                if (pi.PropertyType == typeof(string))
                {
                    var value = (string) pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value)) return true;
                }

            return false;
        }
    }
}