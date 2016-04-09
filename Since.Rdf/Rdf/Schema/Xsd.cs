using System;
using Since.Collections;
using Since.Net;
using System.Collections.Generic;

namespace Since.Rdf.Schema
{
    public class Xsd
    {
        public static readonly string Prefix = "xsd";
        public static readonly string Namespace = "http://www.w3.org/2001/XMLSchema#";

        private static readonly BiDictionary<Type, Iri> LiteralTypeMap = new BiDictionary<Type, Iri>
        {
            {typeof (bool),             new Iri(Xsd.Namespace + "boolean")},
            {typeof (double),           new Iri(Xsd.Namespace + "double")},
            {typeof (float),            new Iri(Xsd.Namespace + "float")},
            {typeof (DateTime),         new Iri(Xsd.Namespace + "dateTime")},
            {typeof (DateTimeOffset),   new Iri(Xsd.Namespace + "dateTimeStamp")},
            {typeof (TimeSpan),         new Iri(Xsd.Namespace + "duration")},
            {typeof (sbyte),            new Iri(Xsd.Namespace + "byte")},
            {typeof (short),            new Iri(Xsd.Namespace + "short")},
            {typeof (int),              new Iri(Xsd.Namespace + "int")},
            {typeof (long),             new Iri(Xsd.Namespace + "long")},
            {typeof (byte),             new Iri(Xsd.Namespace + "unsignedByte")},
            {typeof (ushort),           new Iri(Xsd.Namespace + "unsignedShort")},
            {typeof (uint),             new Iri(Xsd.Namespace + "unsignedInt")},
            {typeof (ulong),            new Iri(Xsd.Namespace + "unsigndLong")},
            {typeof (Uri),              new Iri(Xsd.Namespace + "anyURI")}
        };

        public static bool TryGetDataType(Type type, out Iri dataType)
        {
            return Xsd.LiteralTypeMap.TryGetByFirst(type, out dataType);
        }

        public static bool TryGetType(Iri dataType, out Type type)
        {
            return Xsd.LiteralTypeMap.TryGetBySecond(dataType, out type);
        }
        
        private static Dictionary<Type, Func<string, object>>  FromString = new Dictionary<Type, Func<string, object>>
                {
                    [typeof (bool)]             = s => Boolean.Parse(s),
                    [typeof (double)]           = s => Double.Parse(s),
                    [typeof (float)]            = s => Single.Parse(s),
                    [typeof (DateTime)]         = s => DateTime.Parse(s),
                    [typeof (DateTimeOffset)]   = s => DateTimeOffset.Parse(s),
                    [typeof (TimeSpan)]         = s => TimeSpan.Parse(s),
                    [typeof (sbyte)]            = s => SByte.Parse(s),
                    [typeof (short)]            = s => Int16.Parse(s),
                    [typeof (int)]              = s => Int32.Parse(s),
                    [typeof (long)]             = s => Int64.Parse(s),
                    [typeof (byte)]             = s => Byte.Parse(s),
                    [typeof (ushort)]           = s => UInt16.Parse(s),
                    [typeof (uint)]             = s => UInt32.Parse(s),
                    [typeof (ulong)]            = s => UInt64.Parse(s),
                    [typeof (Uri)]              = s => new Uri(s)
                };

        public static bool TryGetValueFromString<T>(string value, Iri dataType, out T obj)
        {
            try
            {
                obj = (T)FromString[typeof(T)](value);
                return true;
            }
            catch (Exception)
            {
                obj = default(T);
                return false;
            }
        }

        public static bool TryGetValueFromString(string value, Iri dataType, out object obj)
        {
            obj = null;

            Type type;
            if (!Xsd.TryGetType(dataType, out type))
                return false;
            
            try
            {
                obj = FromString[type](value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}