namespace Minsk.CodeAnalysis.Symbols
{
	//Right now there is only one use case
	//(seperator.vertical() -> (Container, Container)
	//And maybe that will be removed into one type.
	//So idk.
	public class TupleTypeSymbol : TypeSymbol
	{
		public TupleTypeSymbol(TypeSymbol[] children) : base($"Tuple({string.Join<TypeSymbol>(", ", children)})")
		{
			Children = children;
			Length = Children.Length;
		}

		public override TypeType Type => TypeType.Tuple;

		//TODO(Debate): Huh?!
		//I mean it's kinda data type. 
		//On the other hand, if you have two container
		//in it, then those will be added to the slide
		//No questions asked.

		//Actually completely depends on the fact, if
		//you can store a tuple into a variable.
		//As of now you can't. So it doesn't matter..
		public override bool IsData => true;

		public override bool AllowsNone => false;

		public int Length { get; }
		public TypeSymbol[] Children { get; }

		//TODO: We don't even know if we want to keep the tuple type
		//so why should we make a little complex implementation here?
		public override bool HasDefaultValue => throw new System.NotImplementedException();

		public override object DefaultValue => throw new System.NotImplementedException();
	}
}