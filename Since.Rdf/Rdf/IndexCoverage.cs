using System;
using System.Diagnostics;
using Since.Rdf;

namespace Since.Rdf
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum TripleMask
    {
        None = 0,

        Subject = 1,
        Predicate = 2,
        Object = 4,
        Context = 8,

        All = TripleMask.Subject | TripleMask.Predicate | TripleMask.Object | TripleMask.Context
    }
    
    /// <summary>
    /// Number of indices = 2^4 - 2
    /// </summary>
    [Flags]
    public enum IndexCoverage : ushort
    {
        None = 0,

        Subject = 0x0001, // 0b0001
        Predicate = 0x0002, // 0b0010
        Object = 0x0004, // 0b0100
        Context = 0x0008, // 0b1000

        SubjectPredicate = 0x0010, // 0b0011
        SubjectObject = 0x0020, // 0b0101
        SubjectContext = 0x0040, // 0b1001
        PredicateObject = 0x0080, // 0b0110
        PredicateContext = 0x0100, // 0b1010
        ObjectContext = 0x0200, // 0b1100

        SubjectPredicateObject = 0x0400, // 0b0111
        SubjectPredicateContext = 0x0800, // 0b1011
        SubjectObjectContext = 0x1000, // 0b1101
        PredicateObjectContext = 0x2000, // 0b1110

        Single = IndexCoverage.Subject | IndexCoverage.Predicate | IndexCoverage.Object | IndexCoverage.Context,

        Double =
            IndexCoverage.SubjectPredicate | IndexCoverage.SubjectObject | IndexCoverage.SubjectContext |
            IndexCoverage.PredicateObject | IndexCoverage.PredicateContext | IndexCoverage.ObjectContext,

        Triple =
            IndexCoverage.SubjectPredicateObject | IndexCoverage.SubjectPredicateContext |
            IndexCoverage.SubjectObjectContext | IndexCoverage.PredicateObjectContext,

        Basic = IndexCoverage.Single,
        Default = IndexCoverage.Basic,
        Full = ushort.MaxValue
    }

    /// <summary>
    /// 
    /// </summary>
    public static class IndexCoverageExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static TripleMask ToMask(this IndexCoverage ic)
        {
            Debug.Assert((ic & (ic - 1)) == 0, "Only one flag can be set.");

            byte[] table = {1, 2, 4, 8, 3, 5, 9, 6, 10, 12, 7, 11, 13, 14};
            return (TripleMask) table[(int) Math.Log((int) ic, 2)];
        }

        // Selects the best index in the supplied IndexCoverage for the given set.
        // Returns a tuple<index, remaining bitfield>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static Tuple<IndexCoverage, TripleMask> SelectIndex(this IndexCoverage coverage, TripleMask mask)
        {
            if (mask == TripleMask.None)
                return Tuple.Create(IndexCoverage.None, TripleMask.None);

            // The prefered order of indices.
            byte[] pref = {10, 13, 12, 11, 5, 4, 7, 6, 8, 9, 0, 2, 1, 3};

            // The bitmask for each index.
            byte[] table = {1, 2, 4, 8, 3, 5, 9, 6, 10, 12, 7, 11, 13, 14};

            int i;
            for (i = 0; i < 14; i++)
                if (((int) mask & table[pref[i]]) == table[pref[i]] && coverage.HasFlag((IndexCoverage) (1 << pref[i])))
                    break;

            return Tuple.Create((IndexCoverage) (1 << pref[i]), (TripleMask) ((int) mask & ~table[pref[i]]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="o"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Tuple<IndexCoverage, TripleMask> SelectIndex(this IndexCoverage coverage, INode s, INode p,
            INode o, INode c)
        {
            var requested = TripleMask.None;

            if (s != null) requested |= TripleMask.Subject;
            if (p != null) requested |= TripleMask.Predicate;
            if (o != null) requested |= TripleMask.Object;
            if (c != null) requested |= TripleMask.Context;

            return coverage.SelectIndex(requested);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coverage"></param>
        /// <param name="triple"></param>
        /// <returns></returns>
        public static Tuple<IndexCoverage, TripleMask> SelectIndex(this IndexCoverage coverage, Edge triple)
            => coverage.SelectIndex(triple.Subject, triple.Predicate, triple.Object, triple.Context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coverage"></param>
        /// <returns></returns>
        public static bool HasSingleFlag(this IndexCoverage coverage)
        {
            var x = (int) coverage;
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool EqualsWithMask(this Edge a, Edge b, TripleMask mask)
        {
            return (!mask.HasFlag(TripleMask.Subject) || a.Subject.Equals(b.Subject))
                   && (!mask.HasFlag(TripleMask.Predicate) || a.Predicate.Equals(b.Predicate))
                   && (!mask.HasFlag(TripleMask.Object) || a.Object.Equals(b.Object))
                   && (!mask.HasFlag(TripleMask.Context) || a.Context.Equals(b.Context));
        }
    }
}