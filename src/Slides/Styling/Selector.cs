using System;
using System.Collections.Generic;

namespace Slides.Styling
{
	public class Selector
	{
		private Selector(SelectorKind kind, string name, Selector child)
		{
			Kind = kind;
			Name = name;
			Child = child;
		}

		public SelectorKind Kind { get; }
		public string Name { get; }
		public Selector Child { get; private set; }


		public void AddField(string name)
		{
			if (Kind == SelectorKind.All) throw new Exception();
			if (Child != null)
				Child.AddField(name);
			else
				Child = new Selector(SelectorKind.Field, name, null);
		}
		public static Selector CreateAll() => new Selector(SelectorKind.All, null, null);
		public static Selector CreateType(string name) => new Selector(SelectorKind.Type, name, null);
		public static Selector CreateCustom(string name) => new Selector(SelectorKind.Custom, name, null);

		public override bool Equals(object obj)
		{
			if (obj is Selector s)
			{
				return ToString() == s.ToString();
			}
			return false;
		}

		public override string ToString()
		{
			if (Kind == SelectorKind.All) return "*";
			return $"{(Kind == SelectorKind.Field ? ">" : "")}{Kind}:{Name}{Child?.ToString()}";
		}

		public override int GetHashCode() => ToString().GetHashCode();

		public static bool operator ==(Selector left, Selector right) => EqualityComparer<Selector>.Default.Equals(left, right);

		public static bool operator !=(Selector left, Selector right) => !(left == right);
	}
}
