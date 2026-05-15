using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniScope
{
	public class ResourceResolver : IResolver
	{
		public bool CanResolve(Type type)
			=> type.IsSubclassOf(typeof(Object));

		public bool TryResolve(IScope scope, Type type, out object value)
		{
			var path = type.ToString().Replace('.', '/');
			var asset = Resources.Load(path, type);
			value = asset;
			return asset;
		}

		public void Dissolve(object value)
		{
			if (value is Object uValue)
				Resources.UnloadAsset(uValue);
			else
				Debug.LogException(new Exception($"Inconsistent type while dissolving {value} of type {value.GetType()}"));
		}
	}
}