using System;

namespace UniScope
{
	public interface IResolver
	{
		bool CanResolve(Type type);
		bool TryResolve(IScope scope, Type type, out object value);
		void Dissolve(object value);
	}

	public static class ResolverUtility
	{
		public static bool CanResolve<T>(this IResolver resolver)
			=> resolver.CanResolve(typeof(T));

		public static bool TryResolve<T>(this IResolver resolver, IScope scope, out T value)
			where T : class
			=> (value = resolver.TryResolve(scope, typeof(T), out var o) ? o as T : null) != null;

		public static T Resolve<T>(this IResolver resolver, IScope scope)
			where T : class
			=> resolver.TryResolve(scope, typeof(T), out var o) ? (T)o : throw new CannotResolveException(scope, typeof(T));
	}
}