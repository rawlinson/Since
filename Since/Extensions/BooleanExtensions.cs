
namespace Since.Extensions
{
    public static class BooleanExtensions
    {
        /// <summary>
        /// Returns the supplied value if the specified predicate is true.
        /// </summary>
        /// <typeparam name="T">The object type of <paramref name="value"/>.</typeparam>
        /// <param name="predicate">A boolean to test for true.</param>
        /// <param name="value">The value to return if <paramref name="predicate"/> is <see langword="true"/>.</param>
        /// <returns><paramref name="value" /> if <paramref name="predicate"/> is <see langword="true"/>; otherwise <c>default(T)</c>.</returns>
        public static T Then<T>(this bool predicate, T value)
            => predicate ? value : default(T);
    }
}
