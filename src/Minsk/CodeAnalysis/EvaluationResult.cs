using Minsk.CodeAnalysis.SlidesTypes;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Minsk.CodeAnalysis
{
	public sealed class EvaluationResult
	{
		public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object value, TimeWatcher timewatch)
		{
			Diagnostics = diagnostics;
			Value = value;
			Timewatch = timewatch;
		}

		public ImmutableArray<Diagnostic> Diagnostics { get; }
		public object Value { get; }
		public TimeWatcher Timewatch { get; }

	}
}