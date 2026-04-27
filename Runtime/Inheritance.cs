using System;

namespace UniScope
{
	[Flags]
	public enum Inheritance : byte
	{
		Self = 0x01,
		Interfaces = 0x02,
		Bases = 0x04
	}
}