using UnityEngine;

namespace UniScope
{
	public class InjectedBehaviour : MonoBehaviour
	{
		protected virtual void Start()
			=> this.Inject();
	}
}