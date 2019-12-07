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

		public override string ToString()
		{
			return $"{A} - {B}";
		}
	}

}
