using System;
using System.Linq;

namespace UniScope
{
	public class ConstructResolver : IResolver
	{
		public bool CanResolve(Type type)
			=> type.IsClass && !type.IsAbstract;

		public bool TryResolve(IScope registry, Type type, out object value)
		{
			foreach (var (constructor, parameters) in type.GetConstructors().Select(c => (c, c.GetParameters())).OrderByDescending(cp => cp.Item2.Length))
			{
				var arguments = new object[parameters.Length];
				var canConstruct = true;

				for (var i = 0; i < parameters.Length; i++)
				{
					var parameterType = parameters[i].ParameterType;
					if (registry.TryResolve(parameterType, out var argument))
						arguments[i] = argument;
					else
					{
						canConstruct = false;
						break;
					}
				}

				if (canConstruct)
				{
					value = constructor.Invoke(arguments);
					return true;
				}
			}

			value = null;
			return false;
		}

		public void Dissolve(object value)
			=> GC.SuppressFinalize(value);
	}
}