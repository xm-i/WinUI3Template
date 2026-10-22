using AppTemplate.Composition.Interfaces.Services;
using AppTemplate.Composition.Stores.State.Model;
using AppTemplate.Store.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace AppTemplate.Core.Tests;

public class StateStoreTests : IDisposable {
	private readonly string _tempDirectory;
	private readonly ServiceProvider _serviceProvider;
	private readonly Mock<IAppPathProvider> _pathProviderMock;

	public StateStoreTests() {
		this._tempDirectory = Path.Combine(Path.GetTempPath(), "StateStoreTest_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(this._tempDirectory);

		var services = new ServiceCollection();
		services.AddSingleton<ILogger<StateStore>>(NullLogger<StateStore>.Instance);
		services.AddTransient<AppStateModel>();
		services.AddTransient<WindowStateModel>();

		this._pathProviderMock = new Mock<IAppPathProvider>();
		this._pathProviderMock.Setup(x => x.StateFilePath).Returns(Path.Combine(this._tempDirectory, "test.states"));

		services.AddSingleton(this._pathProviderMock.Object);

		this._serviceProvider = services.BuildServiceProvider();
	}

	[Fact]
	public void Load_WhenFileDoesNotExist_CreatesDefaultState() {
		using var store = new StateStore(this._serviceProvider, this._pathProviderMock.Object);

		store.State.ShouldNotBeNull();
		store.State.WindowState.Width.Value.ShouldBe(1000);
		store.State.WindowState.Height.Value.ShouldBe(700);
		store.State.WindowState.IsMaximized.Value.ShouldBeFalse();
	}

	[Fact]
	public void SaveAndLoad_PersistsStateValuesCorrectly() {
		var stateFilePath = Path.Combine(this._tempDirectory, "save_test.states");
		this._pathProviderMock.Setup(x => x.StateFilePath).Returns(stateFilePath);

		using (var store = new StateStore(this._serviceProvider, this._pathProviderMock.Object)) {
			store.State.WindowState.Width.Value = 1280;
			store.State.WindowState.Height.Value = 800;
			store.State.WindowState.IsMaximized.Value = true;
			store.Save();
		}

		File.Exists(stateFilePath).ShouldBeTrue();

		using (var reloadStore = new StateStore(this._serviceProvider, this._pathProviderMock.Object)) {
			reloadStore.State.WindowState.Width.Value.ShouldBe(1280);
			reloadStore.State.WindowState.Height.Value.ShouldBe(800);
			reloadStore.State.WindowState.IsMaximized.Value.ShouldBeTrue();
		}
	}

	public void Dispose() {
		this._serviceProvider.Dispose();
		if (Directory.Exists(this._tempDirectory)) {
			Directory.Delete(this._tempDirectory, true);
		}
	}
}