using System;

namespace Since
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ImmutableAttribute : Attribute
    {
        public ImmutableAttribute()
        { }

        public bool Trusted { get; set; }
    }
}
