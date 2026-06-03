using UnityEngine;

namespace UniScope
{
	public class ScopedBehaviour : MonoBehaviour
	{
		public IScope Scope { get; private set; }

		protected virtual Inheritance RegistrationFlags => Inheritance.Self;

		protected virtual void Awake()
		{
			Scope = this.ResolveScope();
			Scope.Register(this, RegistrationFlags);
		}

		protected virtual void Start()
		{
			this.Inject();
		}
	}
}