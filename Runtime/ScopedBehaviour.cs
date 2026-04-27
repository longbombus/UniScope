using UnityEngine;

namespace UniScope
{
	public class ScopedBehaviour : MonoBehaviour
	{
		public IScope Scope { get; private set; }

		protected virtual Inheritance RegistrationFlags => Inheritance.Self;

		private void Awake()
		{
			Scope = this.ResolveScope();
			Scope.Register(this, RegistrationFlags);

			OnAwake();
		}

		protected virtual void OnAwake()
		{
		}
	}
}