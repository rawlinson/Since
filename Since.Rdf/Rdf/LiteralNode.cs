using System;
using System.Text;
using Since.Net;
using Since.Rdf.Schema;

namespace Since.Rdf
{
    [Immutable]
    public class LiteralNode : Node, IEquatable<LiteralNode>
    {
        public LiteralNode(string plainLiteral)
            : this(plainLiteral, null, Rdf.Schema.Rdf.PlainLiteral.Iri)
        { }

        public LiteralNode(string plainLiteral, string language)
            : this(plainLiteral, language, Rdf.Schema.Rdf.PlainLiteral.Iri)
        { }

        public LiteralNode(string typedLiteral, Iri dataType)
            : this(typedLiteral, null, dataType)
        { }

        private LiteralNode(string literal, string language, Iri dataType)
        {
            this.Value = literal;
            this.Language = language?.ToLower();
            this.DataType = dataType;
        }
        
        public string Value { get; }

        public string Language { get; }

        public Iri DataType { get; }

        public bool Equals(LiteralNode other)
            => other != null
               && (
                   ReferenceEquals(this, other)
                || this.Value.Equals(other.Value)
               );

        public override bool Equals(object other)
            => this.Equals(other as LiteralNode);

        public override int GetHashCode()
            => new {this.Value, this.DataType, this.Language}.GetHashCode();

        public override string ToString()
        {
            var result = $"\"{this.Value}\"";
            
            if (!string.IsNullOrEmpty(this.Language))
                result += $"@{this.Language}";
            else if (this.DataType != null)
                result += $"^^{this.DataType}";
            return result;
        }

        public bool TryGetValue<T>(out T obj)
        {
            return Xsd.TryGetValueFromString(this.Value, this.DataType, out obj);
        }

        public bool TryGetValue(out object obj)
        {
            return Xsd.TryGetValueFromString(this.Value, this.DataType, out obj);
        }
    }
}