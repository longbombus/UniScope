using System.Collections.Generic;

namespace UniScope
{
	public interface IUniqueRegistry
	{
		public bool TryGet<T>(out T value);
	}

	public interface IMultiRegistry
	{
		public bool TryGet<T>(out IReadOnlyCollection<T> value);
	}

	public interface INamedUniqueRegistry
	{
		public bool TryGet<T>(string name, out T value);
	}
}