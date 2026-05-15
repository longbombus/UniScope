using System.Reflection;

namespace UniScope
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class InjectAttribute : System.Attribute
	{
		public readonly bool IsRequired;

		public InjectAttribute()
			=> IsRequired = true;

		public InjectAttribute(object @null)
			=> IsRequired = false;
	}

	public static class InjectUtility
	{
		public static void Inject(this object contextAndTarget)
			=> ScopeUtility.ResolveScope(contextAndTarget).Inject(contextAndTarget);

		public static void Inject(this IScope scope, object target)
		{
			const BindingFlags memberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			for (var type = target.GetType(); !type.IsRootType(); type = type.BaseType)
			{
				foreach (var field in type.GetFields(memberFlags))
				{
					var inject = field.GetCustomAttribute<InjectAttribute>();
					if (inject == null)
						continue;

					if (scope.TryResolve(field.FieldType, out object value))
						field.SetValue(target, value);
					else if (inject.IsRequired)
						throw new CannotResolveException(scope, field.FieldType, field.Name);
				}
			}
		}
	}
}