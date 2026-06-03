using System;

namespace UniScope
{
	[Flags]
	public enum Relation
	{
		Parent  = 0b_0000_0001,
		Sibling = 0b_0000_0010,
		Child   = 0b_0000_0100,
	}
}