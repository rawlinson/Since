using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Since.Rdf.ObjectModel
{
    public abstract class RdfObject
        : ObservableDynamic<RdfObject.Dict, INode, object>
    {
        public RdfObject(INode node, IEdgeSet edgeSet = default(IEdgeSet))
            : base(new Dict(node, edgeSet))
        {
            var test = base._items as Dict;

            this.Node = test.Node;
            this.EdgeSet = test.EdgeSet;
        }

        public INode Node { get; }

        public IEdgeSet EdgeSet { get; }

        protected static INode GetAboutNodeForValue<T>(T value, Type type)
        {
            PropertyInfo _;
            var about = RdfAboutAttribute.Get(type, out _);
            if (String.IsNullOrWhiteSpace(about?.IriFormatString))
                return null;

            return new IriNode(String.Format(about.IriFormatString, value));
        }

        protected static string GetValueOfAboutNode(INode node, Type type)
        {
            PropertyInfo _;
            var about = RdfAboutAttribute.Get(type, out _);
            if (String.IsNullOrWhiteSpace(about?.IriFormatString))
                return null;

            return RdfAboutAttribute.Unformat(about.IriFormatString, node.ToString());
        }

        /// <summary>
        /// Gets the <see cref="RdfClassAttribute" /> of the object.
        /// </summary>
        public RdfClassAttribute GetClassAttribute()
            => RdfClassAttribute.Get(this.GetType());
   
        /// <summary>
        /// Gets the <see cref="RdfPropertyAttribute" /> of the specified property.
        /// </summary>
        public RdfPropertyAttribute GetPropertyAttribute(PropertyInfo property)
            => RdfPropertyAttribute.Get(this.GetType(), property);

        /// <summary>
        /// Gets the <see cref="RdfAboutAttribute" /> of the object.
        /// </summary>
        public RdfAboutAttribute GetAboutAttribute(out PropertyInfo property)
            => RdfAboutAttribute.Get(this.GetType(), out property);

        protected void SetProperty<T>(T value, [CallerMemberName]string name = null)
        {
            throw new NotImplementedException();
        }

        protected void SetListProperty<T>(IEnumerable<T> value)
        {
            throw new NotImplementedException();
        }

        protected T GetProperty<T>([CallerMemberName]string name = null)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<T> GetListProperty<T>()
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetKey(string name, out INode result)
        {
            throw new NotImplementedException();
        }

        protected override bool TryGetName(INode key, out string result)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return base.TrySetMember(binder, value);
        }
        
        public class Dict : INotifyCollectionChanged, IDictionary<INode, object>
        {
            public Dict(INode node, IEdgeSet edgeSet = default(IEdgeSet))
            {
                this.Node = node ?? new BlankNode();
                this.EdgeSet = edgeSet ?? new IndexedEdgeSet(IndexCoverage.SubjectPredicate);
            }

            public INode Node { get; }
            public IEdgeSet EdgeSet { get; }

            public ICollection<INode> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<object> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public object this[INode key]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public void Add(INode key, object value)
            {
                CollectionChanged?.Invoke(this, null);
                throw new NotImplementedException();
            }

            public bool ContainsKey(INode key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(INode key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(INode key, out object value)
            {
                throw new NotImplementedException();
            }

            public void Add(KeyValuePair<INode, object> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<INode, object> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<INode, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<INode, object> item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<INode, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
