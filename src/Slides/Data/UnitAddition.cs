using System;

namespace Slides.Data
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

		internal override Unit GetMaxComponent() => Max(A, B);
		public override bool ContainsPercent()
		{
			return A.ContainsPercent() || B.ContainsPercent();
		}
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

		public override int h_ToPixel(int referenceSize)
		{
			return A.h_ToPixel(referenceSize) + B.h_ToPixel(referenceSize);
		}

		public static UnitAddition operator *(float a, UnitAddition b) => new UnitAddition(b.A * a, b.B * a);
		public static UnitAddition operator *(UnitAddition a, float b) => b * a;
		public static UnitAddition operator /(UnitAddition a, float b) => new UnitAddition(a.A / b, a.B / b);

		public override string ToString()
		{
			return $"{A} + {B}";
		}
	}

}
