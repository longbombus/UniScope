using UnityEngine;

namespace UniScope
{
	[SelectionBase]
	public sealed class GameObjectScope : MonoBehaviour, IScope
	{
		public IScope Parent { get; private set; }
		public Registry Registry { get; } = new Registry();

		private void Awake()
		{
			Parent = this.ResolveScope();
			Registry.Register<GameObjectScope, IScope>(this);
		}
	}
}