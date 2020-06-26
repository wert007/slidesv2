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



		public static UnitAddition operator *(float a, UnitAddition b) => new UnitAddition(b.A * a, b.B * a);
		public static UnitAddition operator *(UnitAddition a, float b) => b * a;
		public static UnitAddition operator /(UnitAddition a, float b) => new UnitAddition(a.A / b, a.B / b);

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


		public static UnitSubtraction operator *(float a, UnitSubtraction b) => new UnitSubtraction(b.A * a, b.B * a);
		public static UnitSubtraction operator *(UnitSubtraction a, float b) => b * a;
		public static UnitSubtraction operator /(UnitSubtraction a, float b) => new UnitSubtraction(a.A / b, a.B / b);

		public Unit Add(Unit u)
		{
			if (A.Kind == u.Kind)
				return new UnitSubtraction(A + u, B);
			if (Kind == u.Kind) //TODO: Try to simplify!
				return new UnitAddition(this, u);
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
