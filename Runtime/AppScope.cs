using System.Linq;
using UnityEngine;

namespace UniScope
{
	[CreateAssetMenu(fileName = "AppScope", menuName = "Scopes/AppScope")]
	public class AppScope : ScriptableObject, IScope
	{
		public IScope Parent => null;
		public Registry Registry { get; } =  new Registry();

		[Header("Register")]
		[SerializeField] private Inheritance registrationFlags;
		[SerializeField] private ScriptableObject[] assets;
		// TODO: make editor that can add whole Folders of assets, see UnityEditor.DefaultAsset

		public static AppScope Instance { get; private set; }

		private void OnEnable()
		{
			RegisterAll();
			Instance = this;
		}

		private void RegisterAll()
		{
			foreach (var asset in assets)
				this.Register(asset, registrationFlags);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets();
			if (!preloadedAssets.Contains(this))
			{
				System.Array.Resize(ref preloadedAssets, preloadedAssets.Length + 1);
				preloadedAssets[^1] = this;
				UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets);
			}
		}
#endif
	}
}