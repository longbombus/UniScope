using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace UniScope
{
	[Flags]
	public enum Inheritance : byte
	{
		Self = 0x01,
		Interfaces = 0x02,
		Bases = 0x04
	}

	public static class InheritanceUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRootType([NotNullWhen(false)] this Type? type)
			=> type == null
			|| type == typeof(object)
			|| type == typeof(ScopedBehaviour)
			|| type == typeof(MonoBehaviour)
			|| type == typeof(ScriptableObject);
	}
}