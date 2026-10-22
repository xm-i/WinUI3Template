using AppTemplate.Composition.Enum;
using AppTemplate.Composition.Interfaces.Services;
using AppTemplate.Composition.Stores.Config.Model;
using AppTemplate.Store.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace AppTemplate.Core.Tests;

public class ConfigStoreTests : IDisposable {
	private readonly string _tempDirectory;
	private readonly ServiceProvider _serviceProvider;
	private readonly Mock<IAppPathProvider> _pathProviderMock;

	public ConfigStoreTests() {
		this._tempDirectory = Path.Combine(Path.GetTempPath(), "ConfigStoreTest_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(this._tempDirectory);

		var services = new ServiceCollection();
		services.AddSingleton<ILogger<ConfigStore>>(NullLogger<ConfigStore>.Instance);
		services.AddTransient<AppConfigModel>();
		services.AddTransient<LanguageConfigModel>();
		services.AddTransient<EnvironmentConfigModel>();

		this._pathProviderMock = new Mock<IAppPathProvider>();
		this._pathProviderMock.Setup(x => x.ConfigFilePath).Returns(Path.Combine(this._tempDirectory, "test.config"));

		services.AddSingleton(this._pathProviderMock.Object);

		this._serviceProvider = services.BuildServiceProvider();
	}

	[Fact]
	public void Load_WhenFileDoesNotExist_CreatesDefaultConfig() {
		using var store = new ConfigStore(this._serviceProvider, this._pathProviderMock.Object);

		store.Config.ShouldNotBeNull();
		store.Config.Version.ShouldBe(1);
		store.Config.LanguageConfig.Language.Value.ShouldBe(string.Empty);
		store.Config.EnvironmentConfig.Theme.Value.ShouldBe(AppTheme.Default);
	}

	[Fact]
	public void SaveAndLoad_PersistsConfigValuesCorrectly() {
		var configFilePath = Path.Combine(this._tempDirectory, "save_test.config");
		this._pathProviderMock.Setup(x => x.ConfigFilePath).Returns(configFilePath);

		using (var store = new ConfigStore(this._serviceProvider, this._pathProviderMock.Object)) {
			store.Config.LanguageConfig.Language.Value = "ja-JP";
			store.Config.EnvironmentConfig.Theme.Value = AppTheme.Dark;
			store.Save();
		}

		File.Exists(configFilePath).ShouldBeTrue();

		using (var reloadStore = new ConfigStore(this._serviceProvider, this._pathProviderMock.Object)) {
			reloadStore.Config.LanguageConfig.Language.Value.ShouldBe("ja-JP");
			reloadStore.Config.EnvironmentConfig.Theme.Value = AppTheme.Dark;
		}
	}

	public void Dispose() {
		this._serviceProvider.Dispose();
		if (Directory.Exists(this._tempDirectory)) {
			Directory.Delete(this._tempDirectory, true);
		}
	}
}