using UnityEngine;

namespace UniScope
{
	[System.Serializable]
	public class RegistrationArgs<T>
	{
		[SerializeField] private Inheritance flags;
		[SerializeField] private T instance;

		public Inheritance Flags => flags;
		public T Instance => instance;
	}
}