using System.Linq;
using TypeDropdown;
using UnityEngine;

namespace UniScope
{
	[CreateAssetMenu(fileName = "AppScope", menuName = "Scopes/AppScope")]
	public class AppScope : ScriptableObject, IScope
	{
		public IScope Parent => null;
		public Registry Registry { get; } = new Registry();

		[Header("Register")]
		[SerializeField] private RegistrationArgs<ScriptableObject>[] assets;
		// TODO: make editor that can add whole Folders of assets, see UnityEditor.DefaultAsset
#if ENABLED_TYPEDROPDOWN
		[TypeDropdown]
#endif
		[SerializeReference] private IResolver[] resolvers;

		public static AppScope Instance { get; private set; }

		private void OnEnable()
		{
			RegisterAll();
			Instance = this;
		}

		private void RegisterAll()
		{
			Registry.Register<AppScope, IScope>(this);

			foreach (var asset in assets)
				this.Register(asset.Instance, asset.Flags);

			foreach (var resolver in resolvers)
				Registry.Register(typeof(IResolver), resolver);
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

			if (resolvers == null || resolvers.Length == 0)
				resolvers = new IResolver[]
				{
					new ResourceResolver(),
					new ConstructResolver(),
				};
		}
#endif
	}
}