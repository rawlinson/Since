using Since.Rdf;
using Since.Rdf.ObjectModel;

namespace Since.Rdf.Generator
{
    [RdfClass(Iri = "river:Dummy", BaseIri = "river:Dummy#")]
    public partial class Dummy : RdfObject
    {
        public Dummy(INode node, IEdgeSet edgeSet = null)
            : base(node, edgeSet)
        { }
        
        [RdfProperty(Iri = "name")]
        public string Name { get { return this.GetProperty<string>(); } set { this.SetProperty(value); } }
    }
}
