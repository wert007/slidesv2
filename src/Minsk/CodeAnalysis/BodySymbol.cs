using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis
{
	[Serializable]
	public class SerializableBodySymbol
	{
		public VariableSymbol Symbol { get; }
		public string Body { get; }
		public SerializableLibrarySymbol Source { get; internal set; }

		public SerializableBodySymbol(BodySymbol body)
		{
			Symbol = body.Symbol;
			if(body.Body != null)
				Body = Serializer.Serialize(body.Body);
		}

		public BodySymbol ToBody(LibrarySymbol[] references)
		{
			BoundBlockStatement body = null;
			if(Body != null)
			{
				var deserializer = new Deserializer(Body, references);
				body = (BoundBlockStatement)deserializer.Deserialize();
			}
			return new BodySymbol(Symbol, body);
		}
	}
	public class BodySymbol
	{
		//TODO: Maybe use TypeSymbol instead of VariableSymbol
		internal BodySymbol(VariableSymbol symbol, BoundBlockStatement body)
		{
			Symbol = symbol;
			Body = body;
		}
		internal BodySymbol(TypeSymbol symbol, BoundBlockStatement body)
		{
			var variable = new VariableSymbol(symbol.Name, true, symbol, symbol.IsData);
			Symbol = variable;
			Body = body;
		}

		public VariableSymbol Symbol { get; }
		internal BoundBlockStatement Body { get; }
		public LibrarySymbol Source { get; set; }

		public override string ToString() => Symbol.ToString();
	}
}