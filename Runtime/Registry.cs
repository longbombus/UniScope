using System;
using System.Collections.Generic;

namespace UniScope
{
	public class Registry
		: IUniqueRegistry
		, IMultiRegistry
		, IDisposable
	{
		private readonly Dictionary<Type, object> uniqueRegistry = new();
		private Dictionary<Type, List<object>> multiRegistry = new();

		public void Dispose()
		{
			uniqueRegistry.Clear();
		}

		public void Register(Type type, object instance)
		{
			if (multiRegistry != null && multiRegistry.TryGetValue(type, out var listed))
			{
				listed.Add(instance);
			}
			else if (!uniqueRegistry.TryAdd(type, instance) && uniqueRegistry.Remove(type, out var oldObject))
			{
				multiRegistry ??= new Dictionary<Type, List<object>>();
				if (!multiRegistry.TryGetValue(type,  out listed))
					multiRegistry.Add(type, listed = new List<object>());

				listed.Add(oldObject);
				listed.Add(instance);
			}
		}

		public bool TryGet<T>(out T value)
		{
			if (uniqueRegistry.TryGetValue(typeof(T), out object o))
			{
				value = (T)o;
				return true;
			}

			if (multiRegistry.TryGetValue(typeof(T), out var listed) && listed.Count > 0)
			{
				value = (T)listed[0];
				return true;
			}

			value = default;
			return false;
		}

		public bool TryGet<T>(out IReadOnlyCollection<T> value)
		{
			if (multiRegistry.TryGetValue(typeof(T), out var listed) && listed.Count > 0)
			{
				value = (IReadOnlyCollection<T>)listed;
				return true;
			}

			if (uniqueRegistry.TryGetValue(typeof(T), out var o))
			{
				value = new T[1]{ (T)o };
				return true;
			}

			value = null;
			return false;
		}
	}
}