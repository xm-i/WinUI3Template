using AppTemplate.Composition.Constants;
using AppTemplate.Composition.Interfaces.Stores;
using AppTemplate.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using R3;
using Serilog;

namespace AppTemplate;

/// <summary>
/// アプリケーションのエントリポイントおよびライフサイクル管理クラス
/// </summary>
public partial class App : Application {
	private readonly IConfigStore _configStore;
	private readonly IStateStore _stateStore;
	private MainWindow? _mainWindow;

	/// <summary>
	/// <see cref="App"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	public App() {
		if (!Directory.Exists(FilePathConstants.BaseDirectory)) {
			Directory.CreateDirectory(FilePathConstants.BaseDirectory);
		}

		BuildConfigureServices();

		this._configStore = Ioc.Default.GetRequiredService<IConfigStore>();
		this._stateStore = Ioc.Default.GetRequiredService<IStateStore>();

		// 言語設定を UI 初期化よりも前に適用する
		var language = this._configStore.Config.LanguageConfig.Language.Value;
#if UNPACKAGED
		Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = string.IsNullOrEmpty(language) ? "en" : language;
#else
		Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = string.IsNullOrEmpty(language) ? string.Empty : language;
#endif

		this.InitializeComponent();

		// プロセス終了時に設定と状態を永続化する
		AppDomain.CurrentDomain.ProcessExit += (_, _) => {
			this._stateStore.Save();
			this._configStore.Save();
		};
	}

	/// <summary>
	/// アプリケーションが起動されたときに呼び出されます。
	/// </summary>
	/// <param name="args">起動引数</param>
	protected override void OnLaunched(LaunchActivatedEventArgs args) {
		var logger = Ioc.Default.GetRequiredService<ILogger<App>>();

		// 未処理例外ハンドラーの登録
		AppDomain.CurrentDomain.UnhandledException += (_, e) => {
			logger.LogError(e.ExceptionObject as Exception, "AppDomain UnhandledException");
		};
		this.UnhandledException += (_, e) => {
			logger.LogError(e.Exception, "XAML UnhandledException");
		};
		ObservableSystem.RegisterUnhandledExceptionHandler(e => {
			logger.LogError(e, "R3 UnhandledException");
		});

		// メインウィンドウの表示
		this._mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
		this._mainWindow.Activate();

		logger.LogInformation("アプリケーションが正常に起動しました。");
	}

	/// <summary>
	/// DI コンテナとロギングの構成を行います。
	/// </summary>
	private static void BuildConfigureServices() {
		string[] logFields = [
			"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}",
			"{Level:u4}",
			"{ThreadId:00}",
			"{Message:j}",
			"{SourceContext}",
			"{NewLine}{Exception}"
		];

		Log.Logger = new LoggerConfiguration()
			.Enrich.WithThreadId()
#if DEBUG
			.MinimumLevel.Verbose()
#else
			.MinimumLevel.Information()
#endif
			.WriteTo.Debug(outputTemplate: string.Join("｜", logFields))
			.WriteTo.File(
				Path.Combine(FilePathConstants.LogDirectoryPath, "app-.log"),
				rollingInterval: RollingInterval.Day,
				outputTemplate: string.Join("\t", logFields))
			.CreateLogger();

		var serviceCollection = new ServiceCollection();
		serviceCollection.AddLogging(loggingBuilder => {
			loggingBuilder.AddSerilog(dispose: true);
		});

		// 各プロジェクトの AutoDiAttributes による自動登録メソッドを呼び出す
		serviceCollection.AddGeneratedServices();
		Composition.DIRegistration.AddGeneratedServices(serviceCollection);
		Core.DIRegistration.AddGeneratedServices(serviceCollection);
		Store.DIRegistration.AddGeneratedServices(serviceCollection);
		ViewModels.DIRegistration.AddGeneratedServices(serviceCollection);

		Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
	}
}