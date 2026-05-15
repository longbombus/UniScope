using System.Collections.Generic;
using NUnit.Framework;

namespace UniScope.Tests
{
	public class ScopeTests
	{
		private class A
		{
		}

		private class B : A
		{
		}

		[Test]
		public void ScopeSingleResolution()
		{
			var scope = new Scope();

			var a = new A();
			var b = new B();

			scope.Registry.Register(a);
			scope.Registry.Register(b);

			Assert.AreSame(a, scope.Resolve<A>());
			Assert.AreSame(b, scope.Resolve<B>());
		}


		[Test]
		public void ScopeMultipleResolution()
		{
			var scope = new Scope();

			var a = new A();
			var b = new B();

			scope.Registry.Register<A>(a);
			scope.Registry.Register<A>(b);

			Assert.That(scope.Resolve<A>() == a || scope.Resolve<A>() == b);
			Assert.NotNull(scope.Resolve<IEnumerable<A>>());
			Assert.NotNull(scope.Resolve<IReadOnlyCollection<A>>());
			Assert.NotNull(scope.Resolve<IReadOnlyList<A>>());

			var instances = scope.Resolve<List<A>>();
			Assert.That(instances.Contains(a));
			Assert.That(instances.Contains(b));
		}

		[Test]
		public void ScopeHierarchicalResolution()
		{
			var parent = new Scope();
			var child = new Scope(parent);

			var a = new A();

			parent.Register(a);

			Assert.NotNull(parent.Resolve<A>());
			Assert.NotNull(child.Resolve<A>());
			Assert.AreSame(parent.Resolve<A>(), child.Resolve<A>());
		}
	}
}