using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Since
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
        Inherited = false, AllowMultiple = false)]
    public sealed class ImmutableAttribute : Attribute
    {
        public ImmutableAttribute()
        { }

        public bool Trusted { get; set; }
    }
}
