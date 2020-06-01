using System;

namespace Slides
{
	[Serializable]
	public class UnitAddition : Unit
	{
		public UnitAddition(Unit a, Unit b) : base(a.Value + b.Value, UnitKind.Addition)
		{
			A = a;
			B = b;
		}

		public Unit A { get; }
		public Unit B { get; }

		protected override Unit GetMaxComponent() => Max(A, B);

		public Unit Add(Unit u)
		{
			if (A.Kind == u.Kind)
				return new UnitAddition(A + u, B);
			return new UnitAddition(A, B + u);
		}
		//TODO: Precedence
		public Unit Subtract(Unit u)
		{
			if (A.Kind == u.Kind)
				return new UnitAddition(A - u, B);
			return new UnitAddition(A, B - u);
		}

		public override string ToString()
		{
			return $"{A} + {B}";
		}
	}
	
	public class UnitSubtraction : Unit
	{
		public UnitSubtraction(Unit a, Unit b) : base(a.Value - b.Value, UnitKind.Subtraction)
		{
			A = a;
			B = b;
		}

		public Unit A { get; }
		public Unit B { get; }
		protected override Unit GetMaxComponent() => Max(A, B);

		public Unit Add(Unit u)
		{
			if (A.Kind == u.Kind)
				return new UnitSubtraction(A + u, B);
			return new UnitSubtraction(A, B + u);
		}
		//TODO: Precedence
		public Unit Subtract(Unit u)
		{
			if (A.Kind == u.Kind)
				return new UnitSubtraction(A - u, B);
			return new UnitSubtraction(A, B - u);
		}
		public override string ToString()
		{
			if(B.Kind == UnitKind.Subtraction ||
				B.Kind == UnitKind.Addition)
				return $"{A} - ({B})";
			return $"{A} - {B}";
		}
	}

}
