using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Since.Net;
using System.Diagnostics.Contracts;

namespace Since.Rdf
{

    /*
    public static class ObjectExtensions
    {
        private static readonly Dictionary<Type, string> LiteralTypeMap = new Dictionary<Type, string>
        {
            {typeof (bool), "xsd:boolean"},
            {typeof (double), "xsd:double"},
            {typeof (float), "xsd:float"},
            {typeof (DateTime), "xsd:dateTime"},
            {typeof (DateTimeOffset), "xsd:dateTimeStamp"},
            {typeof (TimeSpan), "xsd:duration"},
            {typeof (sbyte), "xsd:byte"},
            {typeof (short), "xsd:short"},
            {typeof (int), "xsd:int"},
            {typeof (long), "xsd:long"},
            {typeof (byte), "xsd:unsignedByte"},
            {typeof (ushort), "xsd:unsignedShort"},
            {typeof (uint), "xsd:unsignedInt"},
            {typeof (ulong), "xsd:unsigndLong"},
            {typeof (Uri), "xsd:anyURI"}
        };


        private static bool TryGetLiteral<T>(T obj, out INode literal)
        {
            literal = null;

            var value = obj.ToString();
            string dataType;
            if (obj is string)
                literal = new LiteralNode(value);
            else if (ObjectExtensions.LiteralTypeMap.TryGetValue(obj.GetType(), out dataType))
                literal = new LiteralNode(value, dataType);
            else
                return false;

            return true;
        }

        private static Type GetObjectType<T>(this T property, out Type enclosingType)
        {
            var type = property.GetType();
            enclosingType = null;

            var typeInfo = type.GetTypeInfo();
            if (type.IsArray)
            {
                type = type.GetElementType();
                enclosingType = typeof (Array);
            }
            else if (typeInfo.IsGenericType &&
                     typeof (List<>).GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition().GetTypeInfo()))
            {
                type = typeInfo.GenericTypeArguments[0];
                enclosingType = typeof (List<>);
            }

            return type;
        }

        private static IriNode GetResource(MemberInfo property)
        {
            return new IriNode(new Iri(
                property.GetCustomAttribute<ResourceAttribute>()?.Iri
                ?? $"assembly:{property.Module}/{property.DeclaringType.FullName.Replace('.', '/')}#{property.Name}"
                ));
        }

        private static IriNode GetResource(TypeInfo type)
        {
            return new IriNode(new Iri(
                type.GetCustomAttribute<ResourceAttribute>()?.Iri
                ?? $"assembly:{type.Module}/{type.FullName.Replace('.', '/')}"
                ));
        }


        private static void DoSave(INode s, INode p, object o, ICollection<Edge> edges)
        {
            if (o == null)
                return;
            INode literal;
            if (ObjectExtensions.TryGetLiteral(o, out literal))
                edges.Add(new Edge(s, p, literal));
            else
                edges.Add(new Edge(s, p, ObjectExtensions.GetEdges(o, edges)));
        }

        public static IEnumerable<Edge> GetEdges<T>(this T obj)
        {
            Contract.Requires(obj != null);

            var edges = new List<Edge>();
            ObjectExtensions.GetEdges(obj, edges);
            return edges;
        }

        private static INode GetEdges<T>(T obj, ICollection<Edge> edges)
        {
            Type enclosingType;
            var type = obj.GetObjectType(out enclosingType);

            if (enclosingType != null)
            {
                var blank = new BlankNode();
                foreach (var child in (IEnumerable)obj)
                    ObjectExtensions.DoSave(blank, ObjectExtensions.RdfsMemberNode, child, edges);
                return blank;
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            INode subject = null;
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<AboutAttribute>() != null)
                    subject = new IriNode(new Iri(property.GetValue(obj).ToString()));
            }
            subject = subject ?? new BlankNode();

            edges.Add(new Edge(subject, ObjectExtensions.RdfTypeNode,
                ObjectExtensions.GetResource(type.GetTypeInfo())));

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<AboutAttribute>() == null)
                    ObjectExtensions.DoSave(subject, ObjectExtensions.GetResource(property), property.GetValue(obj),
                        edges);
            }

            return subject;
        }
    }
    */
}