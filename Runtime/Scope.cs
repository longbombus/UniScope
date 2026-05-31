using System;
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
			Registry.Register<Scope, IScope>(this);
		}
	}

	public static class ScopeUtility
	{
		public static void Register<T>(this IScope scope)
			=> scope.Registry.Register(typeof(T));

		public static void Register<T>(this IScope scope, T instance)
			where T : class
			=> scope.Registry.Register<T>(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register<T>(this IScope scope, T instance, Inheritance flags)
			where T : class
		{
			if (instance.GetType() != typeof(T))
				scope.Registry.Register(typeof(T), instance);

			Register(scope, (object)instance, flags);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Register(this IScope scope, object instance, Inheritance flags)
		{
			var instanceType = instance.GetType();

			if ((flags & Inheritance.Self) != 0)
				scope.Registry.Register(instanceType, instance);

			if ((flags & Inheritance.Bases) != 0)
				for (var type = instanceType.BaseType; !type.IsRootType(); type = type.BaseType)
					scope.Registry.Register(type, instance);

			if ((flags & Inheritance.Interfaces) != 0)
				foreach (var i in instanceType.GetInterfaces())
					scope.Registry.Register(i, instance);
		}

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
		public static bool TryResolve(this object context, Type type, out object instance)
			=> ResolveScope(context).TryResolve(type, out instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryResolve(this IScope scope, Type type, out object instance)
		{
			for (var s = scope; s != null; s = s.Parent)
				if (s.Registry.TryResolve(type, out instance))
					return true;

			instance = null;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryResolve<T>(this object context, out T instance)
			where T : class
			=> ResolveScope(context).TryResolve(out instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryResolve<T>(this IScope scope, out T instance)
			where T : class
			=> (instance = scope.TryResolve(typeof(T), out var obj) ? obj as T : null) != null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Resolve<T>(this object context)
			where T : class
			=> ResolveScope(context).Resolve<T>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Resolve<T>(this IScope scope)
			where T : class
			=> scope.TryResolve(out T instance)
				? instance
				: throw new CannotResolveException(scope, typeof(T));

		public static IScope GetRoot(this IScope scope)
		{
			while (scope.Parent != null)
				scope = scope.Parent;
			return scope;
		}
	}
}