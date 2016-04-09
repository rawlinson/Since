using System;
using System.Diagnostics.Contracts;
using Since.Net;

namespace Since.Rdf
{
    public class IriNode : Node, IEquatable<IriNode>
    {
        public IriNode(Iri iri)
        {
            Contract.Requires(iri != null);
            this.Iri = iri;
        }

        public Iri Iri { get; }

        public bool Equals(IriNode other)
            => other != null
               && (
                   ReferenceEquals(this, other)
                   || this.Iri.Equals(other.Iri)
               );

        public override bool Equals(object other)
            => this.Equals(other as IriNode);

        public override int GetHashCode()
            => new { this.Iri }.GetHashCode();

        public override string ToString()
            => this.Iri.ToString();
    }
}
