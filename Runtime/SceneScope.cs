using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniScope
{
	public partial class SceneScope : IScope
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

		static SceneScope()
		{
			SceneManager.sceneUnloaded += CleanSceneRegistry;
		}

		[OnExitingPlayMode]
		private static void Destruct()
		{
			Instances.Clear();
			SceneManager.sceneUnloaded += CleanSceneRegistry;
		}

		private static void CleanSceneRegistry(Scene scene)
		{
			if (Instances.TryGetValue(scene, out var sceneScope))
				sceneScope.Registry.Clear();
		}
	}
}