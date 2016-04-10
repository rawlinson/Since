namespace Since
{
    /// <summary>
    /// Defines a generalized method that a value type or class implements to create a type-specific method for determining matching of instances.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Matchable")]
    public interface IMatchable<in T>
    {
        /// <summary>
        /// Indicates whether the current object matches another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object matches the <paramref name="other"/> parameter; otherwise, <see langword="false" />.</returns>
        bool Matches(T other);
    }
}
