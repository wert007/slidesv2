using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Symbols;
using Slides;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	[Serializable]
	public class SerializableLibrarySymbol
	{
		public string Name { get; }
		public SerializableLibrarySymbol[] Referenced { get; }
		public VariableSymbol[] GlobalVariablesKeys { get; }
		public object[] GlobalVariablesValues { get; }
		public FunctionSymbol[] GlobalFunctions { get; }
		public SerializableBodySymbol[] CustomTypes { get; }

		public Style[] Styles { get; }
		public string[] Imports { get; }
		public string[] ReflectionMethodsNames { get; }

		public SerializableLibrarySymbol(LibrarySymbol library)
		{
			Name = library.Name;
			Referenced = new SerializableLibrarySymbol[library.Libraries.Length];
			for (int i = 0; i < Referenced.Length; i++)
			{
				Referenced[i] = new SerializableLibrarySymbol(library.Libraries[i]);
			}
			CustomTypes = new SerializableBodySymbol[library.CustomTypes.Length];
			for (int i = 0; i < CustomTypes.Length; i++)
			{
				CustomTypes[i] = new SerializableBodySymbol(library.CustomTypes[i]);
				CustomTypes[i].Source = this;
			}
			GlobalFunctions = library.GlobalFunctions;
			GlobalVariablesKeys = new VariableSymbol[library.GlobalVariables.Count];
			GlobalVariablesValues = new object[library.GlobalVariables.Count];
			for(int i = 0; i < GlobalVariablesKeys.Length; i++)
			{
				GlobalVariablesKeys[i] = library.GlobalVariables.ElementAt(i).Key;
				GlobalVariablesValues[i] = library.GlobalVariables.ElementAt(i).Value;
			}
			Styles = library.Styles;
			Imports = library.Imports;
			ReflectionMethodsNames = library.GlobalFunctionsReflectionsNames;
		}

		public LibrarySymbol ToLibrarySymbol()
		{
			var libraries = new LibrarySymbol[Referenced.Length];
			for (int i = 0; i < libraries.Length; i++)
				libraries[i] = Referenced[i].ToLibrarySymbol();
			var customTypes = new BodySymbol[CustomTypes.Length];
			for (int i = 0; i < customTypes.Length; i++)
			{
				customTypes[i] = CustomTypes[i].ToBody(libraries);
				TypeSymbol.RegisterType(customTypes[i].Symbol.Type);
			}
			var globalVariablesDict = new Dictionary<VariableSymbol, object>();
			for (int i = 0; i < GlobalVariablesKeys.Length; i++)
			{
				globalVariablesDict.Add(GlobalVariablesKeys[i], GlobalVariablesValues[i]);
			}
			var globalVariables = new VariableValueCollection(globalVariablesDict);
			var result = new LibrarySymbol(Name, libraries, customTypes, Styles, globalVariables, GlobalFunctions, ReflectionMethodsNames, Imports);
			for (int i = 0; i < customTypes.Length; i++)
			{
				customTypes[i].Source = result;
			}
			return result;
		}
	}
}
