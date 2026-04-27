using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UniScope
{
	public class SceneScope : IScope
	{
		public IScope Parent => AppScope.Instance;
		public Registry Registry { get; } = new Registry();

		private static readonly Dictionary<Scene, SceneScope> Instances = new();

		public static SceneScope GetInstance(in Scene scene)
		{
			if (!Instances.TryGetValue(scene, out var instance))
				Instances.Add(scene, instance = new SceneScope());

			return instance;
		}
	}
}