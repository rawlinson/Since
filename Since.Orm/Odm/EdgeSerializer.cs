using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Since.Net;
using Since.Rdf;
using Since.Rdf.Schema;

namespace Since.Odm
{
    public static class exteasfdrn
    {
        public static bool IsDefaultForType<T>(this T obj)
            => EqualityComparer<T>.Default.Equals(obj, default(T));
    } 
    
    public class EdgeSerializer
    {
        public EdgeSerializer(ISchemaMap schemaMap = default(ISchemaMap))
        {
            this.Map = schemaMap ?? new AttributeSchemaMap();
        }

        public ISchemaMap Map { get; }

        private bool TryGetLiteral<T>(T obj, out INode literal)
        {
            literal = null;

            var value = obj.ToString();
            Iri dataType;
            if (obj is string)
                literal = new LiteralNode(value);
            else if (Xsd.TryGetDataType(obj.GetType(), out dataType))
                literal = new LiteralNode(value, dataType);
            else
                return false;

            return true;
        }

        private Type GetObjectType<T>(T property, out Type enclosedType)
        {
            var type = property.GetType();
            enclosedType = null;

            var typeInfo = type.GetTypeInfo();
            if (type.IsArray)
                enclosedType = type.GetElementType();
            else if (typeInfo.IsGenericType &&
                     typeof (List<>).GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition().GetTypeInfo()))
                enclosedType = typeInfo.GenericTypeArguments[0];

            return type;
        }

        private void AddProperty(INode s, INode p, object o, ICollection<Edge> edges)
        {
            if (o == null)
                return;

            INode literal;
            if (this.TryGetLiteral(o, out literal))
                edges.Add(new Edge(s, p, literal));
            else
                edges.Add(new Edge(s, p, this.GetEdges(o, edges)));
        }

        public IEnumerable<Edge> Serialize<T>(T obj)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(Contract.Result<IEnumerable<Edge>>() != null);

            var edges = new List<Edge>();
            this.GetEdges(obj, edges);
            return edges;
        }

        private INode GetEnumerableEdges<T>(T obj, ICollection<Edge> edges)
        {
            Contract.Requires(obj is IEnumerable);

            var blank = new BlankNode();
            foreach (var child in (IEnumerable) obj)
                this.AddProperty(blank, Rdfs.Member, child, edges);
            return blank;
        }

        private INode GetEdges<T>(T obj, ICollection<Edge> edges)
        {
            Type enclosedType;
            var type = this.GetObjectType(obj, out enclosedType);
            if (enclosedType != null)
                return this.GetEnumerableEdges(obj, edges);

            var subject = this.Map.GetAboutNode(type, obj);

            var typeNode = this.Map.GetClassNode(type);
            if (typeNode != null)
                edges.Add(new Edge(subject, Rdf.Schema.Rdf.Type, typeNode));

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (this.Map.IncludeProperty(property))
                {
                    this.AddProperty(subject, this.Map.GetPropertyNode(property), property.GetValue(obj),
                        edges);
                }
            }
            
            return subject;
        }
        
        object GetObjectFor(INode subject, IEdgeSet edges)
        {
            object result = null;

            var typeEdge = edges.AllWith(subject: subject, predicate: Rdf.Schema.Rdf.Type).FirstOrDefault();
            if (typeEdge != null)
            {
                var typeName = this.Map.GetTypeName(typeEdge.Object);
                if (typeName != null)
                {
                    var type = Type.GetType(typeName, throwOnError: false, ignoreCase: true);
                    if (type != null)
                        result = Activator.CreateInstance(type);
                }
            }
            return result ?? new ExpandoObject();
        }

        private void Deserialize(INode subject, IEdgeSet edges, ref object obj, bool throwOnMissing = false)
        {
            var literal = subject as LiteralNode;
            if (literal != null)
            {
                if (literal.TryGetValue(out obj))
                    return;
            }

            if (obj == null)
                obj = this.GetObjectFor(subject, edges);

            var propertyEdges = edges.AllWith(subject: subject);
            foreach (var propertyEdge in propertyEdges)
            {
                if (propertyEdge.Predicate.Equals(Rdf.Schema.Rdf.Type))
                    continue;

                string propertyName;
                if (!this.Map.GetPropertyName(propertyEdge.Predicate, out propertyName))
                {
                    if (throwOnMissing)
                        throw new MemberAccessException("Can not get property for predicate " + propertyEdge.Predicate);
                    continue;
                }

                var property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (property == null || property.CanWrite == false)
                {
                    if (throwOnMissing)
                        throw new MemberAccessException("Can not set property " + propertyName);
                    continue;
                }

                object value = null;
                this.Deserialize(propertyEdge.Object, edges, ref value);

                try
                {
                    property.SetValue(obj, value);
                }
                catch (Exception e) when (e is ArgumentException || e is MethodAccessException || e is TargetInvocationException)
                {
                    if (throwOnMissing)
                        throw;
                }
            }
        }

        public object Deserialize(INode subject, IEdgeSet edges)
        {
            Contract.Requires(subject != null);
            Contract.Requires(subject.IsResource == true);
            Contract.Requires(edges != null);

            object result = null;
            this.Deserialize(subject, edges, ref result);
            return result;
        }

        public T Deserialize<T>(INode subject, IEdgeSet edges)
            where T : class
        {
            Contract.Requires(subject != null);
            Contract.Requires(subject.IsResource == true);
            Contract.Requires(edges != null);

            var typeEdge = edges.AllWith(subject: subject, predicate: Rdf.Schema.Rdf.Type).FirstOrDefault();
            if (typeEdge == null)
                return null;

            var typeName = this.Map.GetTypeName(typeEdge.Object);
            if (typeName == null)
                return null;

            var type = Type.GetType(typeName);
            if (type == null)
                return null;

            if (type != typeof(T) && typeof(T).GetTypeInfo().IsSubclassOf(type))
                return null;

            object result = default(T);
            this.Deserialize(subject, edges, ref result);
            return result as T;
        }
    }
}