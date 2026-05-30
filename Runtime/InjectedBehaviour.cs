using UnityEngine;

namespace UniScope
{
	public class InjectedBehaviour : MonoBehaviour
	{
		private void Start()
		{
			this.Inject();
			OnStart();
		}

		protected virtual void OnStart()
		{
		}
	}
}