using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Since.Rdf.ObjectModel
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false)]
    public class RdfAboutAttribute : Attribute
    {
        public string IriFormatString { get; set; }

        public RdfAboutAttribute(string iriFormatString)
        {
            this.IriFormatString = iriFormatString;
        }

        /// <summary>
        /// Gets the <see cref="RdfAboutAttribute" /> for the specified object.
        /// </summary>
        public static RdfAboutAttribute Get(Type type, out PropertyInfo property)
        {
            Contract.Requires(type != null);

            var classAttribute = RdfClassAttribute.Get(type);
            if (classAttribute != null && !String.IsNullOrWhiteSpace(classAttribute.About))
                return Get(type, classAttribute.About, out property);
            property = null;
            return null;
        }

        /// <summary>
        /// Gets the <see cref="RdfAboutAttribute" /> for the specified property on the supplied object.
        /// </summary>
        public static RdfAboutAttribute Get(Type type, string propertyName, out PropertyInfo property)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyName != null);

            property = type.GetRuntimeProperty(propertyName);
            return property?.GetCustomAttribute<RdfAboutAttribute>();
        }

        /// <summary>
        /// Returns the contents of the first format item.
        /// </summary>
        /// <remarks>Only one format item is allowed in the format string.</remarks>
        public static string Unformat(string format, string str)
        {
            var startIndex = format.IndexOf('{');
            var stopIndex = format.IndexOf('}', startIndex);

            int length = str.Length - (format.Length - stopIndex) - startIndex;

            return str.Substring(startIndex, length);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false)]
    public class RdfClassAttribute : Attribute
    {
        public string Iri { get; set; }
        public string About { get; set; }
        public string BaseIri { get; set; }

        /// <summary>
        /// Gets the <see cref="RdfClassAttribute" /> for the specfied object.
        /// </summary>
        public static RdfClassAttribute Get(Type type)
        {
            Contract.Requires(type != null);

            return type.GetTypeInfo().GetCustomAttribute<RdfClassAttribute>();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = true, Inherited = true)]
    public class RdfNamespaceAttribute : Attribute
    {
        public string Prefix { get; set; }
        public string Iri { get; set; }

        public RdfNamespaceAttribute(string prefix, string iri)
        {
            this.Prefix = prefix;
            this.Iri = iri;
        }

        /// <summary>
        /// Gets the <see cref="RdfNamespaceAttribute" />s for the specified object.
        /// </summary>
        public static IEnumerable<RdfPropertyAttribute> Get(Type type)
        {
            Contract.Requires(type != null);

            return type.GetTypeInfo().GetCustomAttributes<RdfPropertyAttribute>();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false)]
    public class RdfPropertyAttribute : Attribute
    {
        public string Iri { get; set; }
        public string DataType { get; set; }

        public AutoBool IsLiteral { get; set; } = AutoBool.Auto;
        public bool AllowMultiple { get; set; } = false;

        /// <summary>
        /// Gets the <see cref="RdfPropertyAttribute" /> for the specified property on the supplied object.
        /// </summary>
        public static RdfPropertyAttribute Get(Type type, string propertyName)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyName != null);

            var property = type.GetRuntimeProperty(propertyName);
            return Get(type, property);
        }

        /// <summary>
        /// Gets the <see cref="RdfPropertyAttribute" /> for the specified property on the supplied object.
        /// </summary>
        public static RdfPropertyAttribute Get(Type type, PropertyInfo property)
        {
            Contract.Requires(type != null);
            Contract.Requires(property != null);

            return type.GetTypeInfo().GetCustomAttribute<RdfPropertyAttribute>();
        }
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false)]
    public class RdfArrayAttribute : RdfPropertyAttribute
    {
        public bool Nested { get; set; } = false;

        public string ItemIri { get; set; }

        /// <summary>
        /// Gets the <see cref="RdfArrayAttribute" /> for the specified property on the supplied object.
        /// </summary>
        public new static RdfArrayAttribute Get(Type type, string propertyName)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyName != null);

            var property = type.GetRuntimeProperty(propertyName);
            return Get(type, property);
        }

        /// <summary>
        /// Gets the <see cref="RdfArrayAttribute" /> for the specified property on the supplied object.
        /// </summary>
        public new static RdfArrayAttribute Get(Type type, PropertyInfo property)
        {
            Contract.Requires(type != null);
            Contract.Requires(property != null);

            return type.GetTypeInfo().GetCustomAttribute<RdfArrayAttribute>();
        }
    }
}
