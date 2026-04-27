using System;

namespace UniScope
{
	public class CannotResolveException : Exception
	{
		public CannotResolveException(IScope scope, Type type, string name = null)
			: base($"Cannot resolve '{type}' within scope {scope}")
		{
		}
	}
}