using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Data;
using Slides.Elements;
using Slides.Styling;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public class LibrarySymbol
	{
		public string Name { get; }
		public LibrarySymbol[] Libraries { get; }
		public BodySymbol[] CustomTypes { get; }
		public Style[] Styles { get; }
		public VariableValueCollection GlobalVariables { get; }
		public FunctionCallableCollection GlobalFunctions { get; }
		public string[] Imports { get; }
		public static LibrarySymbol Seperator => GetSeperator();
		public static LibrarySymbol Code => CodeLibrary.GetLibrary();
		public static LibrarySymbol QR => QRLibrary.GetLibrary();
		public Type SourceType { get; set; }

		public LibrarySymbol(string name, LibrarySymbol[] libraries, BodySymbol[] customTypes, Style[] styles, VariableValueCollection globalVariables, string[] imports)
			: this(name, libraries, customTypes, styles, globalVariables, new FunctionCallableCollection(), imports)
		{ }
		public LibrarySymbol(string name, LibrarySymbol[] libraries, BodySymbol[] customTypes, Style[] styles, VariableValueCollection globalVariables, FunctionCallableCollection globalFunctions, string[] imports)
		{
			Name = name;
			Libraries = libraries;
			CustomTypes = customTypes;
			Styles = styles;
			GlobalVariables = globalVariables;
			Imports = imports;
			GlobalFunctions = globalFunctions;
			SourceType = typeof(LibrarySymbol);
		}

		public LibrarySymbol(string name)
		{
			Name = name;
		}

		public override string ToString() => Name;

		//public static SplittedContainer _CreateVerticalSeperator(Unit width)
		//{
		//	return new SplittedContainer()
		//}

		public static SplittedContainer CreateVerticalSeperator(Unit width)
		{
			var result = new SplittedContainer(FlowAxis.Horizontal);
			result.childA.orientation = Orientation.LeftStretch;
			result.childA.width = width;
			result.childB.orientation = Orientation.RightStretch;
			result.childB.width = new Unit(100, Unit.UnitKind.Percent) - width;
			result.orientation = Orientation.Stretch;
			return result;
		}

		private class SeperatorCallable : Callable
		{
			public override object Call(FunctionSymbol function, object[] args)
			{
				return CreateVerticalSeperator((Unit)args[0]);
			}
		}

		public static LibrarySymbol GetSeperator()
		{
			var name = "seperator";
			var libraries = new LibrarySymbol[0];
			var customTypes = new BodySymbol[0];
			var styles = new Style[0];
			var globalVariables = new VariableValueCollection(null);
			var builtInTypes = BuiltInTypes.Instance;
			var globalFunctions = new FunctionCallableCollection(new Dictionary<FunctionSymbol, Callable>{
				{ 
					new FunctionSymbol("vertical", new VariableSymbol("width", true, builtInTypes.LookSymbolUp(typeof(Unit))), builtInTypes.LookSymbolUp(typeof(SplittedContainer))),
					new SeperatorCallable() },
			});
			var imports = new string[0];
			var result = new LibrarySymbol(name, libraries, customTypes, styles, globalVariables, globalFunctions, imports);
			return result;
		}

		public bool TryLookUpFunction(string name, out FunctionSymbol[] functions)
		{
			return GlobalFunctions.TryGetSymbol(name, out functions);
		}

		internal object Call(FunctionSymbol function, object[] args)
		{
			return GlobalFunctions[function].Call(function, args);
		}
	}
}
