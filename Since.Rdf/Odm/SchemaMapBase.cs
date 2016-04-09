using System;
using System.Reflection;
using Since.Rdf;

namespace Since.Odm
{
    public abstract class SchemaMapBase : ISchemaMap
    {
        public virtual INode GetAboutNode<T>(Type type, T obj)
        {
            return new BlankNode();
        }

        public virtual INode GetClassNode(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return new IriNode($"assembly:{typeInfo.Module}/{typeInfo.FullName.Replace('.', '/')}");
        }

        public virtual bool IncludeProperty(PropertyInfo property)
        {
            return true;
        }

        public virtual INode GetPropertyNode(PropertyInfo property)
        {
            return new IriNode($"assembly:{property.Module}/{property.DeclaringType.FullName.Replace('.', '/')}#{property.Name.ToLowerInvariant()}");
        }

        public string GetAboutPropertyName(Type type)
        {
            return null;
        }

        public string GetTypeName(INode node)
        {
            var f =  node as IriNode;
            if (f == null)
                return null;

            var typeName = f.Iri.ToString();
            if (typeName.StartsWith("assembly:") == false)
                return null;

            typeName = typeName.Substring(typeName.IndexOf('/') + 1);
            typeName = typeName.Replace('/', '.');

            return typeName;
        }

        public bool GetPropertyName(INode predicate, out string propertyName)
        {
            propertyName = null;

            var node = predicate as IriNode;
            if (node == null)
                return false;

            var parts = node.Iri.ToString().Split(new[] {'#'}, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;

            propertyName = parts[1];
            return true;
        }
    }
}