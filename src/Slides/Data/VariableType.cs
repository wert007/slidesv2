using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Data
{
	public class VariableType
	{
		public VariableType(object value, int variable)
		{
			Value = value;
			Variable = variable;
		}

		public object Value { get; }
		public int Variable { get; }
	}
}
