using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UniScope
{
	public sealed class Registry : IDisposable
	{
		private readonly Dictionary<Type, Entry> entries = new();

		private IScope scope;
		private IReadOnlyCollection<IResolver> resolvers;

		public int EntriesCount => entries.Count;

		public void Dispose()
			=> Clear();

		public void Register<T>(T instance)
			where T : class
			=> Register(typeof(T), instance, null);

		public void Register<T0, T1>(T0 instance)
			where T0 : class, T1
		{
			Register(typeof(T0), instance, null);
			Register(typeof(T1), instance, null);
		}

		public void Register<T0, T1, T2>(T0 instance)
			where T0 : class, T1, T2
		{
			Register(typeof(T0), instance, null);
			Register(typeof(T1), instance, null);
			Register(typeof(T2), instance, null);
		}

		public void Register(Type type, object instance)
			=> Register(type, instance, null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Register(Type type, object instance, IResolver resolver)
		{
			if (entries.TryGetValue(type, out var oldEntry))
				oldEntry.Add(instance);
			else
				entries.Add(type, new Entry(type, instance, resolver));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryResolve<T>(out T value)
			where T : class
			=> (value = TryResolve(typeof(T), out object o) ? o as T : null) != null;

		public bool TryResolve(Type type, out object value)
		{
			if (TryResolveRegistered(type, out object o))
			{
				value = o;
				return true;
			}

			if (TryResolveUnregistered(type, out o, out var resolver))
			{
				Register(type, o, resolver);
				value = o;
				return true;
			}

			value = null;
			return false;
		}

		private bool TryResolveRegistered<T>(out T value)
			where T : class
			=> (value = TryResolveRegistered(typeof(T), out object o) ? o as T : null) != null;

		private bool TryResolveRegistered(Type type, out object value)
		{
			if (entries.TryGetValue(type, out var entry))
				return entry.TryGetInstance(out value);

			if (type.IsGenericType)
			{
				var genericArguments = type.GetGenericArguments();
				if (genericArguments.Length == 1 && type.IsAssignableFrom(typeof(List<>).MakeGenericType(genericArguments[0])) && entries.TryGetValue(genericArguments[0], out entry))
				{
					entry.GetInstances(out value);
					return true;
				}
			}

			value = null;
			return false;
		}

		private bool TryResolveUnregistered(Type type, out object value, out IResolver usedResolver)
		{
			if ((scope != null || TryResolve(out scope)) && (resolvers != null || scope.GetRoot().Registry.TryResolveRegistered(out resolvers)))
				foreach (var resolver in resolvers)
					if (resolver.CanResolve(type) && resolver.TryResolve(scope, type, out value))
					{
						usedResolver = resolver;
						return true;
					}

			value = null;
			usedResolver = null;
			return false;
		}

		public void Clear()
		{
			foreach (var entry in entries.Values)
				entry.Dispose();

			entries.Clear();
		}

		private class Entry : IDisposable
		{
			private object store;
			private readonly IResolver resolver;

			public readonly Type Type;

			public Entry(Type type, object instance, IResolver resolver = null)
			{
				Type = type;
				store = instance;
				this.resolver = resolver;
			}

			public void Add(object instance)
				=> RequireList().Add(instance);

			public bool TryGetInstance(out object value)
			{
				if (store is IList listedInstances)
					value = listedInstances[0];
				else
					value = store;

				return true;
			}

			public void GetInstances(out object values)
				=> values = RequireList();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private IList RequireList()
			{
				if (store is IList instancesList)
					return instancesList;

				instancesList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(Type));
				instancesList.Add(store);
				store = instancesList;
				return instancesList;
			}

			public void Dispose()
			{
				if (resolver == null)
					return;

				if (store is IList instancesList)
					foreach (var instance in instancesList)
						resolver.Dissolve(instance);
				else
					resolver.Dissolve(store);
			}
		}
	}
}