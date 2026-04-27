using System.Runtime.CompilerServices;
using UnityEngine;

namespace UniScope
{
	public interface IScope
	{
		IScope Parent { get; }
		Registry Registry { get; }
	}

	public class Scope : IScope
	{
		public IScope Parent { get; }
		public Registry Registry { get; }

		public Scope(IScope parent = null)
		{
			Parent = parent;
			Registry = new Registry();
		}
	}

	public static class ScopeUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register<T>(this IScope scope, T instance, Inheritance flags)
			where T : class
		{
			var instanceType = instance.GetType();

			if ((flags & Inheritance.Self) != 0)
				scope.Registry.Register(instanceType, instance);

			if ((flags & Inheritance.Bases) != 0)
				for (var type = instanceType.BaseType; type != null && type != typeof(object) && type != typeof(ScopedBehaviour) && type != typeof(MonoBehaviour); type = type.BaseType)
					scope.Registry.Register(type, instance);

			if ((flags & Inheritance.Interfaces) != 0)
				foreach (var i in instanceType.GetInterfaces())
					scope.Registry.Register(i, instance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGet<T>(this IScope scope, out T instance)
			where T : class
			=> scope.Registry.TryGet(out instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Get<T>(this IScope scope)
			where T : class
			=> scope.Registry.TryGet(out T instance)
				? instance
				: throw new CannotResolveException(scope, typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IScope ResolveScope(this Component component)
			=> (IScope)component.GetComponentInParent<GameObjectScope>()
			?? SceneScope.GetInstance(component.gameObject.scene);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IScope ResolveScope<T>(T context)
			=> context switch
			{
				IScope scope => scope,
				ScopedBehaviour scopeBehaviour => scopeBehaviour.Scope,
				Component component => component.ResolveScope(),
				_ => AppScope.Instance
			};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGet<T>(this object context, out T instance)
			where T : class
			=> ResolveScope(context).TryGet(out instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Get<T>(this object context)
			where T : class
			=> ResolveScope(context).Get<T>();
	}
}