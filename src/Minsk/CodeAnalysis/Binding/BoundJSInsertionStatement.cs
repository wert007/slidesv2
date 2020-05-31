using Slides;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class JSInsertionDependencyCollection : ICollection<JSInsertionDependency>
	{
		List<JSInsertionDependency> _collection;
		public JSInsertionDependencyCollection()
		{
			_collection = new List<JSInsertionDependency>();
		}

		public int Count => ((ICollection<JSInsertionDependency>)_collection).Count;

		public bool IsReadOnly => ((ICollection<JSInsertionDependency>)_collection).IsReadOnly;

		public void Add(JSInsertionDependency item)
		{
			if (item == null) return;
			if (item.Kind == JSInsertionKind.None) return;
			if (item.Kind != JSInsertionKind.Slider && _collection.Any(i => i.Kind == item.Kind)) return;
			if (item.Kind == JSInsertionKind.Slider && _collection.Any(i => i.Value.Equals(item.Value))) return;
			_collection.Add(item);
		}

		public void Clear() => _collection.Clear();

		public bool Contains(JSInsertionDependency item)
		{
			return _collection.Contains(item);
		}

		public void CopyTo(JSInsertionDependency[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

		public IEnumerator<JSInsertionDependency> GetEnumerator() => _collection.GetEnumerator();

		public bool Remove(JSInsertionDependency item) => _collection.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	internal class JSInsertionDependency
	{
		public JSInsertionDependency(BoundExpression value)
		{
			Value = value;
			Kind = value.GetJSInsertionKind();
		}

		public JSInsertionDependency(JSInsertionKind kind)
		{
			Value = null;
			Kind = kind;
		}

		public BoundExpression Value { get; }
		public JSInsertionKind Kind { get; }

		internal static JSInsertionDependency Create(BoundExpression expression)
		{
			if (expression.GetJSInsertionKind() == JSInsertionKind.None) return null;
			return new JSInsertionDependency(expression);
		}
	}
	internal class BoundJSInsertionStatement : BoundStatement
	{
		
		public BoundJSInsertionStatement(JSInsertionDependency[] dependencies, BoundStatement body)
		{
			Dependencies = dependencies;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.JSInsertionStatement;

		public JSInsertionDependency[] Dependencies { get; }
		public BoundStatement Body { get; }
	}
}