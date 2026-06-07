using UnityEngine;

namespace UniScope
{
	public class InjectedBehaviour : MonoBehaviour
	{
		protected virtual void Awake()
			=> InjectUtility.InjectComponents(this);

		protected virtual void Start()
			=> this.ResolveScope().Inject(this);
	}
}