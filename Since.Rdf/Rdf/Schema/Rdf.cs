namespace Since.Rdf.Schema
{
    public class Rdf
    {
        public static readonly string Prefix = "rdf";
        public static readonly string Namespace= "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        // Data Types
        public static readonly IriNode PlainLiteral = new IriNode(Rdf.Namespace + "PlainLiteral");

        // Properties
        public static readonly IriNode Type = new IriNode(Rdf.Namespace + "type");
    }
}
