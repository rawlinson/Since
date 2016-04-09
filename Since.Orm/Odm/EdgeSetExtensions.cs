using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Since.Rdf;

namespace Since.Odm
{
    /// <summary>
    /// CRUD extensions for objects on IEdgeSet.
    /// </summary>
    public static class EdgeSetExtensions
    {
        public static INode Add<T>(this IEdgeSet set, T obj)
        {
            Contract.Requires(set != null);

            var s = new EdgeSerializer();

            var edges = s.Serialize(obj);
            set.AddRange(edges);

            return null;
        }

        public static IEnumerable<INode> AddRange<T>(this IEdgeSet set, IEnumerable<T> objs)
        {
            Contract.Requires(set != null);

            foreach (var obj in objs)
                yield return set.Add<T>(obj);
        }

        public static T Get<T>(this IEdgeSet set, INode subject)
        {
            Contract.Requires(set != null);

            throw new NotImplementedException();
        }

        public static void Update<T>(this IEdgeSet set, T obj, bool removeBlankOrphans = true, bool removeIriOrphans = false)
        {
            Contract.Requires(set != null);

            throw new NotImplementedException();

        }

        public static void Remove<T>(this IEdgeSet set, T obj, bool cascade = true, bool removeBlankOrphans = true, bool removeIriOrphans = false)
        {
            Contract.Requires(set != null);

            throw new NotImplementedException();
        }
    }
}