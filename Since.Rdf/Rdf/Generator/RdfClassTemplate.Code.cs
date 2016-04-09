using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Since.Rdf.Generator
{ 
    public partial class RdfClassTemplate
    {
        public RdfClassTemplate(IEdgeSet schema)
        {
            Contract.Requires(schema != null);

            this.Schema = schema;
        }

        public IEdgeSet Schema { get; }

        private string GetNamespace()
        {
            return "none";
        }

        private IEnumerable<INode> GetClasses()
            => this.Schema.AllWhere(predicate: Rdf.Schema.Rdf.Type, obj: Rdf.Schema.Rdfs.Class)
                    .Select(e => e.Subject);

        private string GetClassName(INode node)
        {
            return "none";
        }

        private string GetBaseIri(INode node)
        {
            return null;
        }

        private IEnumerable<INode> GetProperties(INode classNode)
            => this.Schema.AllWhere(predicate: Rdf.Schema.Rdfs.Domain, obj: classNode)
                    .Select(e => e.Subject);

        private string GetPropertyName(INode node)
        {
            return "none";
        }

        private string GetPropertyType(INode node)
        {
            return "none";
        }
    }
}
