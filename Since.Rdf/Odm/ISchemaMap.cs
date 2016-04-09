using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using Since.Rdf;

namespace Since.Odm
{
    [ContractClass(typeof(SchemaMapContracts))]
    public interface ISchemaMap
    {
        INode GetAboutNode<T>(Type type, T obj);
        INode GetClassNode(Type type);
        bool IncludeProperty(PropertyInfo property);
        INode GetPropertyNode(PropertyInfo property);

        string GetAboutPropertyName(Type type);
        string GetTypeName(INode node);
        bool GetPropertyName(INode predicate, out string propertyName);
    }

    [ContractClassFor(typeof(ISchemaMap))]
    public abstract class SchemaMapContracts : ISchemaMap
    {
        private SchemaMapContracts() {}

        public INode GetAboutNode<T>(Type type, T obj)
        {
            Contract.Requires(type != null);
            Contract.Requires(obj != null);
            Contract.Ensures(Contract.Result<INode>() != null);

            throw new NotImplementedException();
        }

        public INode GetClassNode(Type type)
        {
            Contract.Requires(type != null);

            throw new NotImplementedException();
        }

        public bool IncludeProperty(PropertyInfo property)
        {
            Contract.Requires(property != null);

            throw new NotImplementedException();
        }

        public INode GetPropertyNode(PropertyInfo property)
        {
            Contract.Requires(property != null);
            Contract.Ensures(Contract.Result<INode>() != null);

            throw new NotImplementedException();
        }

        public string GetAboutPropertyName(Type type)
        {
            Contract.Requires(type != null);

            throw new NotImplementedException();
        }

        public string GetTypeName(INode node)
        {
            Contract.Requires(node != null);
            Contract.Requires(node is IriNode);

            throw new NotImplementedException();
        }

        public bool GetPropertyName(INode predicate, out string propertyName)
        {
            Contract.Requires(predicate != null);
            Contract.Requires(predicate is IriNode);

            throw new NotImplementedException();
        }
    }
}