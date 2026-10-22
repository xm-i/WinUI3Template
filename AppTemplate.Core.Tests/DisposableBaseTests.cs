using AppTemplate.Common.Base;
using AppTemplate.Composition.Enum;
using R3;
using Shouldly;

namespace AppTemplate.Core.Tests;

public class DisposableBaseTests {
	private class TestDisposable : DisposableBase {
		public bool CustomResourceDisposed {
			get; private set;
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.CustomResourceDisposed = true;
			}
			base.Dispose(disposing);
		}
	}

	[Fact]
	public void Dispose_DisposesCompositeDisposableAndNotifiesOnDisposed() {
		var instance = new TestDisposable();
		var disposedNotified = false;
		instance.OnDisposed.Subscribe(_ => disposedNotified = true);

		var childDisposed = false;
		Disposable.Create(() => childDisposed = true).AddTo(instance.CompositeDisposable);

		instance.DisposeState.ShouldBe(DisposeState.NotDisposed);

		instance.Dispose();

		instance.DisposeState.ShouldBe(DisposeState.Disposed);
		disposedNotified.ShouldBeTrue();
		childDisposed.ShouldBeTrue();
		instance.CustomResourceDisposed.ShouldBeTrue();
	}

	[Fact]
	public void Dispose_MultipleCalls_AreIdempotent() {
		var instance = new TestDisposable();
		var notifyCount = 0;
		instance.OnDisposed.Subscribe(_ => notifyCount++);

		instance.Dispose();
		instance.Dispose();

		instance.DisposeState.ShouldBe(DisposeState.Disposed);
		notifyCount.ShouldBe(1);
	}
}