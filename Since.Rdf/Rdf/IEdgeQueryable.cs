using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Since.Rdf
{
    [ContractClass(typeof(EdgeQueryableContracts))]
    public interface IEdgeQueryable
    {
        /// <summary>
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        IEnumerable<Edge> AllWhere(Edge edge);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="predicate"></param>
        /// <param name="obj"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<Edge> AllWhere(INode subject = null, INode predicate = null, INode obj = null, INode context = null);
    }


    [ContractClassFor(typeof(IEdgeQueryable))]
    public abstract class EdgeQueryableContracts : IEdgeQueryable
    {
        private EdgeQueryableContracts() { }

        public IEnumerable<Edge> AllWhere(Edge edge)
        {
            Contract.Requires(edge != null);

            throw new NotImplementedException();
        }

        public IEnumerable<Edge> AllWhere(INode subject = null, INode predicate = null, INode obj = null, INode context = null)
        {
            throw new NotImplementedException();
        }
    }
}
