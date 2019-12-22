﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Symbols;
using Slides;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public class LibrarySymbol
	{
		public string Name { get; }
		public LibrarySymbol[] Libraries { get; }
		public BodySymbol[] CustomTypes { get; }
		public Style[] Styles { get; }
		public VariableValueCollection GlobalVariables { get; }
		public FunctionSymbol[] GlobalFunctions { get; }
		public string[] GlobalFunctionsReflectionsNames { get; }
		public string[] Imports { get; }
		public static LibrarySymbol Seperator => GetSeperator();
		public static LibrarySymbol Code => CodeLibrary.GetLibrary();
		public Type SourceType { get; set; }

		public LibrarySymbol(string name, LibrarySymbol[] libraries, BodySymbol[] customTypes, Style[] styles, VariableValueCollection globalVariables, string[] imports)
			: this(name, libraries, customTypes, styles, globalVariables, new FunctionSymbol[0], new string[0], imports)
		{ }
		public LibrarySymbol(string name, LibrarySymbol[] libraries, BodySymbol[] customTypes, Style[] styles, VariableValueCollection globalVariables, FunctionSymbol[] globalFunctions, string[] globalFunctionsReflections, string[] imports)
		{
			Name = name;
			Libraries = libraries;
			CustomTypes = customTypes;
			Styles = styles;
			GlobalVariables = globalVariables;
			Imports = imports;
			if (globalFunctions.Length != globalFunctionsReflections.Length)
				throw new ArgumentOutOfRangeException();
			GlobalFunctions = globalFunctions;
			GlobalFunctionsReflectionsNames = globalFunctionsReflections;
			SourceType = typeof(LibrarySymbol);
		}

		public LibrarySymbol(string name)
		{
			Name = name;
		}

		public override string ToString() => Name;

		public static TupleType CreateVerticalSeperator(Unit width)
		{
			//	Console.WriteLine("HEWWO!!!!!");
			Dictionary<string, object> fields = new Dictionary<string, object>();
			fields.Add("left", new Container()
			{
				orientation = Orientation.LeftStretch,
				width = width,
			});
			fields.Add("right", new Container()
			{
				orientation = Orientation.RightStretch,
				width = new Unit(100, Unit.UnitKind.Percent) - width,
			});
			var result = new TupleType(fields);
			return result;
		}

		public static LibrarySymbol GetSeperator()
		{
			var name = "seperator";
			var libraries = new LibrarySymbol[0];
			var customTypes = new BodySymbol[0];
			var styles = new Style[0];
			var globalVariables = new VariableValueCollection(null);
			var globalFunctions = new FunctionSymbol[]
			{
				new FunctionSymbol("vertical", new VariableSymbol("width", true, TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Unit)), false), new TupleTypeSymbol(
					new TypeSymbol[]
					{
						TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Container)),
						TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Container)),
					})),
			};
			var globalFunctionsReflections = new string[]
			{
				nameof(CreateVerticalSeperator),
			};
			var imports = new string[0];
			var result = new LibrarySymbol(name, libraries, customTypes, styles, globalVariables, globalFunctions, globalFunctionsReflections, imports);
			return result;
		}

		
		public bool TryLookUpFunction(string name, out FunctionSymbol[] functions)
		{
			functions = GlobalFunctions.Where(f => f.Name == name).ToArray();
			return GlobalFunctions.Any(f => f.Name == name);
		}

		public MethodInfo LookMethodInfoUp(FunctionSymbol symbol)
		{
			var index = Array.IndexOf(GlobalFunctions, symbol);
			return SourceType.GetMethod(GlobalFunctionsReflectionsNames[index]);
			//return _globalFunctionsReflections[index];
		}
	}
}
