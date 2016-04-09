using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Reflection;
using Since.Net;

namespace Since.Rdf.Schema
{
    public interface ISchema
    {
        void Add(INode subject, INode predicate, INode obj);
    }

    public class SchemaBuilder
    {
        private ISchema _schema;

        public ISchema ToSchema()
        {
            return _schema;
        }

        public INode AddClass(Type type)
        {
            // Add the class.
            INode classNode = this.GetClassNode(type);
            _schema.Add(classNode, Rdf.Type, Rdfs.Class);

            // Add a possible base type.
            if (type.GetTypeInfo().BaseType is Type baseType)
                _schema.Add(classNode, Rdfs.SubClassOf, this.GetOrAddClass(baseType));

            // Add a possible label.
            if (this.GetClassLabel(type) is var l && String.IsNullOrWhiteSpace(l))
                _schema.Add(classNode, Rdfs.Label, new LiteralNode(l));

            // Add a possible comment.
            if (this.GetClassComment(type) is var c && String.IsNullOrWhiteSpace(c))
                _schema.Add(classNode, Rdfs.Comment, new LiteralNode(c));

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (!field.IsInitOnly)
                    this.AddProperty(classNode, field);
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (property.CanRead && property.CanWrite)
                    this.AddProperty(classNode, property);
            }

            return classNode;
        }

        private INode GetOrAddClass(Type type)
        {
            return AddClass(type);
        }

        private INode AddProperty(INode domainNode, MemberInfo property)
        {
            Contract.Requires(domainNode != null);
            Contract.Requires(property != null);
            Contract.Requires(property is FieldInfo || property is PropertyInfo);

            Type type = property match
                (
                    case FieldInfo f : f.FieldType
                    case PropertyInfo p : p.PropertyType
                    //case * : throw new ArgumentException("Expecting value of type FieldInfo or PropertyInfo", nameof(property))
                );

            INode propertyNode = this.GetPropertyNode(property);

            _schema.Add(propertyNode, Rdf.Type, Rdfs.Property);
            _schema.Add(propertyNode, Rdfs.Domain, domainNode);
            _schema.Add(propertyNode, Rdfs.Range, this.GetClassNode(type));

            if (this.GetPropertyLabel(property) is var l && String.IsNullOrWhiteSpace(l))
                _schema.Add(propertyNode, Rdfs.Label, new LiteralNode(l));

            if (this.GetPropertyComment(property) is var c && String.IsNullOrWhiteSpace(c))
                _schema.Add(propertyNode, Rdfs.Comment, new LiteralNode(c));

            return propertyNode;
        }

        private INode GetClassNode(Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<INode>() != null);

            throw new NotImplementedException();
        }

        private INode GetPropertyNode(MemberInfo property)
        {
            Contract.Requires(property != null);
            Contract.Ensures(Contract.Result<INode>() != null);

            throw new NotImplementedException();
        }
        
        private string GetClassComment(Type type)
            => type.GetTypeInfo().GetCustomAttribute<DisplayAttribute>()?.Description;

        private string GetClassLabel(Type type)
            => type.GetTypeInfo().GetCustomAttribute<DisplayAttribute>()?.Name;

        private string GetPropertyLabel(MemberInfo property)
            => property.GetCustomAttribute<DisplayAttribute>()?.Name;

        private string GetPropertyComment(MemberInfo property)
            => property.GetCustomAttribute<DisplayAttribute>()?.Description;
    }
}
