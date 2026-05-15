using NUnit.Framework;

namespace UniScope.Tests
{
	public class ResolverTests
	{
		public class A
		{
		}

		public class B
		{
		}

		public class C
		{
			public readonly A a;
			public readonly B b;

			public C(A a) => this.a = a;
			public C(B b) => this.b = b;
			public C(A a, B b) => (this.a, this.b) = (a, b);
		}


		[Test]
		public void ParameterlessConstructorResolution()
		{
			var scope = new Scope();
			scope.Register<IResolver>(new ConstructResolver());
			var entriesCount = scope.Registry.EntriesCount;

			Assert.NotNull(scope.Resolve<A>());
			Assert.AreEqual(entriesCount + 1, scope.Registry.EntriesCount);
			Assert.NotNull(scope.Resolve<B>());
			Assert.AreEqual(entriesCount + 2, scope.Registry.EntriesCount);
		}

		[Test]
		public void DependentConstructorResolution()
		{
			var a = new A();
			var b = new B();

			var scopeAB = new Scope();
			scopeAB.Register<IResolver>(new ConstructResolver());
			scopeAB.Register(a);
			scopeAB.Register(b);
			Assert.NotNull(scopeAB.Resolve<C>());
			Assert.AreEqual(a, scopeAB.Resolve<C>().a);
			Assert.AreEqual(b, scopeAB.Resolve<C>().b);

			var scopeEmpty = new Scope();
			scopeEmpty.Register<IResolver>(new ConstructResolver());
			Assert.NotNull(scopeEmpty.Resolve<C>());
			Assert.NotNull(scopeEmpty.Resolve<C>().a);
			Assert.NotNull(scopeEmpty.Resolve<C>().b);
		}
	}
}