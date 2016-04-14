using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Since.Rdf.Test
{
    [TestClass]
    public class EdgeTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            INode subject = new BlankNode();
            INode predicate = new BlankNode();
            INode obj = new BlankNode();
            INode context = new BlankNode();

            Edge edge = new Edge(subject, predicate, obj, context);

            Assert.AreEqual(subject, edge.Subject);
            Assert.AreEqual(predicate, edge.Predicate);
            Assert.AreEqual(obj, edge.Object);
            Assert.AreEqual(context, edge.Context);
        }

        [TestMethod]
        public void With_Single_Correct()
        {
            Edge edgeA = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            INode blank = new BlankNode();
            Edge edgeB = edgeA.With(subject: blank);
            Assert.IsTrue(edgeB.Subject == blank);

            Assert.IsTrue(edgeB.Predicate == edgeA.Predicate);
            Assert.IsTrue(edgeB.Object == edgeA.Object);
            Assert.IsTrue(edgeB.Context == edgeA.Context);

            edgeB = edgeA.With(predicate: blank);
            Assert.IsTrue(edgeB.Predicate == blank);

            edgeB = edgeA.With(obj: blank);
            Assert.IsTrue(edgeB.Object == blank);

            edgeB = edgeA.With(context: blank);
            Assert.IsTrue(edgeB.Context == blank);
        }

        [TestMethod]
        public void Matches_Self_True()
        {
            Edge edgeA = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            Assert.IsTrue(edgeA.Matches(edgeA));
        }

        [TestMethod]
        public void Matches_SingleAnyNode_True()
        {
            Edge edgeA = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            Assert.IsTrue(edgeA.Matches(edgeA.With(subject: new AnyNode())));
            Assert.IsTrue(edgeA.Matches(edgeA.With(predicate: new AnyNode())));
            Assert.IsTrue(edgeA.Matches(edgeA.With(obj: new AnyNode())));
            Assert.IsTrue(edgeA.Matches(edgeA.With(context: new AnyNode())));
        }

        [TestMethod]
        public void Matches_Different_False()
        {
            Edge edgeA = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            Assert.IsFalse(edgeA.Matches(null));
            Assert.IsFalse(edgeA.Matches(edgeA.With(subject: new BlankNode())));
            Assert.IsFalse(edgeA.Matches(edgeA.With(predicate: new BlankNode())));
            Assert.IsFalse(edgeA.Matches(edgeA.With(obj: new BlankNode())));
            Assert.IsFalse(edgeA.Matches(edgeA.With(context: new BlankNode())));
        }

        [TestMethod]
        public void Matches_Static_Correct()
        {
            Edge edge = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            Assert.IsTrue(Edge.Matches(null, null));
            Assert.IsTrue(Edge.Matches(edge, edge));
            Assert.IsFalse(Edge.Matches(edge, null));
            Assert.IsFalse(Edge.Matches(null, edge));
        }

        [TestMethod]
        public void Matches_Static_AnyNode_Correct()
        {
            Edge edgeA = new Edge(new BlankNode(), new BlankNode(), new BlankNode(), new BlankNode());

            Edge edgeB = edgeA.With(predicate: new AnyNode());
            Assert.IsTrue(Edge.Matches(edgeB, edgeA));

            edgeB = new Edge(edgeA.Subject, null, edgeA.Object, edgeA.Context);
            Assert.IsFalse(Edge.Matches(edgeB, edgeA));
            Assert.IsTrue(Edge.Matches(edgeB, edgeB));
        }
    }
}
