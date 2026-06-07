using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UniScope
{
	[AttributeUsage(AttributeTargets.Field)]
	public class InjectAttribute : Attribute
	{
		public readonly bool IsRequired;

		public InjectAttribute()
			=> IsRequired = true;

		public InjectAttribute(object @null)
			=> IsRequired = false;
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class InjectComponentAttribute : Attribute
	{
		public readonly Relation Relation;
		public readonly bool IsRequired;

		public InjectComponentAttribute() : this(Relation.Sibling, true) { }
		public InjectComponentAttribute(object @null) : this(Relation.Sibling, false) { }
		public InjectComponentAttribute(Relation relation) : this(relation, true) { }
		public InjectComponentAttribute(Relation relation, object @null) : this(relation, false) { }

		private InjectComponentAttribute(Relation relation, bool isRequired)
		{
			Relation = relation;
			IsRequired = isRequired;
		}
	}

	public static class InjectUtility
	{
		private const BindingFlags MemberFlags
			= BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Instance
			| BindingFlags.DeclaredOnly;

		public static void Inject(this object contextAndTarget)
			=> ScopeUtility.ResolveScope(contextAndTarget).Inject(contextAndTarget);

		public static void Inject(this Component contextAndTarget)
		{
			contextAndTarget.ResolveScope().Inject(contextAndTarget);
			InjectComponents(contextAndTarget);
		}

		public static void Inject(this IScope scope, object target)
		{
			for (var type = target.GetType(); !type.IsRootType(); type = type.BaseType)
			{
				foreach (var field in type.GetFields(MemberFlags))
				{
					var inject = field.GetCustomAttribute<InjectAttribute>();
					if (inject == null)
						continue;

					if (scope.TryResolve(field.FieldType, out object value))
						field.SetValue(target, value);
					else if (inject.IsRequired)
						throw new CannotResolveException(scope, field.FieldType, field.Name);
				}
			}
		}

		public static void InjectComponents(Component target)
		{
			var gameObject = target.gameObject;
			for (var type = target.GetType(); !type.IsRootType(); type = type.BaseType)
			{
				foreach (var field in type.GetFields(MemberFlags))
				{
					var inject = field.GetCustomAttribute<InjectComponentAttribute>();
					if (inject == null)
						continue;

					var componentType = field.FieldType;
					var component = inject.Relation switch
					{
						Relation.Parent => gameObject.transform.parent?.GetComponent(componentType),
						Relation.Sibling | Relation.Parent => gameObject.GetComponentInParent(componentType),
						Relation.Sibling => gameObject.GetComponent(componentType),
						Relation.Sibling | Relation.Child => gameObject.GetComponentInChildren(componentType),
						Relation.Child => gameObject.GetComponentInChildrenOnly(componentType),
						_ => null
					};

					field.SetValue(target, component);
				}
			}
		}

		private static Component GetComponentInChildrenOnly(this GameObject gameObject, Type componentType)
		{
			var q = new Queue<Transform>();
			{
				var transform = gameObject.transform;
				var childCount = transform.childCount;
				for (int i = 0; i < childCount; ++i)
					q.Enqueue(transform.GetChild(i));
			}

			while (q.TryDequeue(out var childTransform))
			{
				var component = childTransform.GetComponent(componentType);
				if (component)
					return component;

				var subChildCount = childTransform.childCount;
				for (int i = 0; i < subChildCount; ++i)
					q.Enqueue(childTransform.GetChild(i));
			}

			return null;
		}
	}
}