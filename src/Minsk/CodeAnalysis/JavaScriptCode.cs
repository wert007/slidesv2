using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
	public class JavaScriptCode
	{
		public JavaScriptCode(string code, Dictionary<string, string> variableDefinitions)
		{
			Code = code;
			VariableDefinitions = variableDefinitions;
		}

		public string Code { get; }
		public Dictionary<string, string> VariableDefinitions { get; }
	}
}