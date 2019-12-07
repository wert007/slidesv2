using Minsk.CodeAnalysis.SlidesTypes;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
	public sealed class EvaluationResult
	{
		public EvaluationResult(Diagnostic[] diagnostics, object value, TimeWatcher timewatch)
		{
			Diagnostics = diagnostics;
			Value = value;
			Timewatch = timewatch;
		}

		public Diagnostic[] Diagnostics { get; }
		public object Value { get; }
		public TimeWatcher Timewatch { get; }

	}
}