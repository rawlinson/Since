namespace Since.Rdf.Schema
{
    public class Rdfs
    {
        public static readonly string Prefix = "rdfs";
        public static readonly string Namespace = "http://www.w3.org/2000/01/rdf-schema#";

        // Types
        public static readonly IriNode Class = new IriNode(Rdfs.Namespace + "Class");
        public static readonly IriNode Property = new IriNode(Rdfs.Namespace + "Property");

        // Properties
        public static readonly IriNode Comment = new IriNode(Rdfs.Namespace + "comment");
        public static readonly IriNode Domain = new IriNode(Rdfs.Namespace + "domain");
        public static readonly IriNode Label = new IriNode(Rdfs.Namespace + "label");
        public static readonly IriNode Member = new IriNode(Rdfs.Namespace + "member");
        public static readonly IriNode Range = new IriNode(Rdfs.Namespace + "range");
        public static readonly IriNode RdfsSeeAlso = new IriNode(Rdfs.Namespace + "seeAlso");
        public static readonly IriNode SubClassOf = new IriNode(Rdfs.Namespace + "subClassOf");
        
    }
}
