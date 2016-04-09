using System;
using System.Reflection;
using Since.Net;
using Since.Rdf;

namespace Since.Odm
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ResourceAttribute : Attribute
    {
        public string Iri { get; }

        public ResourceAttribute(string iri)
        {
            this.Iri = iri;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AboutAttribute : Attribute
    { }

    public class AttributeSchemaMap : SchemaMapBase
    {
        public override INode GetAboutNode<T>(Type type, T obj)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<AboutAttribute>() != null)
                    return new IriNode(new Iri(property.GetValue(obj).ToString()));
            }

            return base.GetAboutNode(type, obj);
        }

        public override INode GetClassNode(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var attribute = typeInfo.GetCustomAttribute<ResourceAttribute>();
            if (attribute != null)
                return new IriNode(attribute.Iri);

            return base.GetClassNode(type);
        }

        public override bool IncludeProperty(PropertyInfo property)
        {
            return property.GetCustomAttribute<AboutAttribute>() == null;
        }

        public override INode GetPropertyNode(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ResourceAttribute>();
            if (attribute != null)
                return new IriNode(attribute.Iri);

            return base.GetPropertyNode(property);
        }
    }
}