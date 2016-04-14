using System;

namespace Since
{
    /// <summary>
    /// Marks a target as being immutable.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ImmutableAttribute : Attribute
    {
        /// <summary>
        /// Initialzes a new <see cref="ImmutableAttribute"/>.
        /// </summary>
        public ImmutableAttribute()
        { }

        /// <summary>
        /// Gets or sets whether the attribute is trusted.
        /// </summary>
        /// <remarks>
        /// If <see langword="true"/> , the target is not checked for immutability during analysis. This allows classes, properties, etc. to be treated as immutable even if the implemntation allows changes.
        /// </remarks>
        /// <value><see langword="true"/> if the attribute is trust; otherwise, <see langword="false"/>. Default: <see langword="false"/></value>
        public bool Trusted { get; set; }
    }
}
