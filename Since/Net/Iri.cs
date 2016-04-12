using Since.Text;

namespace Since.Net
{
    [Immutable]
    public class Iri : InternedString
    {
        public Iri(string iri)
            : base(iri, Iri.IriRoot)
        { }

        internal Iri(InternedStringNode node)
            : base(node)
        { }

        private static InternedStringNode IriRoot { get; } = new InternedStringNode();

        public override string ToString()
            => this.Node.ToAbsoluteString();
        
        public static implicit operator Iri(string iri)
        {
            return new Iri(iri);
        }
    }
}