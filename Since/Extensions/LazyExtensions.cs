using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Since.Extensions
{
    public static class LazyExtension
    {
        private static Dictionary<Tuple<object, string>, object> _lazyDictionary
            = new Dictionary<Tuple<object, string>, object>();

        /// <summary>
        /// Gets a lazily instantiated value.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="obj">The object that owns the value.</param>
        /// <param name="func">The function to instantiate the value.</param>
        /// <param name="member">The name of the value; <see cref="CallerMemberNAame"/> by default.</param>
        /// <returns>A value of type <see cref="T"/> as returned by <paramref name="func" />.</returns>
        public static T Lazy<T>(this Object obj, Func<T> func, [CallerMemberName]string name = null)
        {
            //Contract.Requires<ArgumentNullException>(func != null);
            //Contract.Requires<ArgumentNullException>(name != null);

            Object value;

            var key = Tuple.Create(obj, name);
            if (!_lazyDictionary.TryGetValue(key, out value))
            {
                value = func.Invoke();
                _lazyDictionary.Add(key, value);
            }

            return (T)value;
        }
    }
}
