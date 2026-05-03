using UnityEngine;

namespace UniScope
{
	public sealed class ScopedRegistrator : MonoBehaviour
	{
		[SerializeField] private RegistrationArgs<Component>[] registrations;

		public IScope Scope { get; private set; }

		private void Awake()
		{
			Scope = this.ResolveScope();

			foreach (var reg in registrations)
				Scope.Register(reg.Instance, reg.Flags);
		}
	}
}