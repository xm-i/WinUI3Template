using AppTemplate.Common.Base;
using AppTemplate.Composition.Interfaces;
using AppTemplate.Composition.Interfaces.Stores;
using Microsoft.Extensions.Logging;

namespace AppTemplate.ViewModels;

/// <summary>
/// メインウィンドウ用の ViewModel。
/// R3 によるリアクティブな状態管理、コマンド、およびリソース破棄を実演します。
/// </summary>
[Inject(InjectServiceLifetime.Transient)]
public class MainWindowViewModel : ViewModelBase {
	private readonly ILogger<MainWindowViewModel> _logger;
	private readonly IStringProvider _stringProvider;

	/// <summary>
	/// アプリケーション設定ストア
	/// </summary>
	public IConfigStore ConfigStore {
		get;
	}

	/// <summary>
	/// アプリケーション状態ストア
	/// </summary>
	public IStateStore StateStore {
		get;
	}

	/// <summary>
	/// クリック回数を保持するリアクティブプロパティ
	/// </summary>
	public BindableReactiveProperty<int> ClickCount {
		get;
	}

	/// <summary>
	/// クリックカウントをインクリメントするリアクティブコマンド
	/// </summary>
	public ReactiveCommand<Unit> IncrementCommand {
		get;
	}

	/// <summary>
	/// ユーザー入力テキストを保持するリアクティブプロパティ
	/// </summary>
	public BindableReactiveProperty<string> InputText {
		get;
	}

	/// <summary>
	/// 入力テキストに基づいてリアクティブに更新される表示文字列
	/// </summary>
	public BindableReactiveProperty<string> OutputText {
		get;
	}

	/// <summary>
	/// <see cref="MainWindowViewModel"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	/// <param name="configStore">設定ストア</param>
	/// <param name="stateStore">状態ストア</param>
	/// <param name="stringProvider">ローカライズ文字列プロバイダー</param>
	/// <param name="logger">ロガー</param>
	public MainWindowViewModel(
		IConfigStore configStore,
		IStateStore stateStore,
		IStringProvider stringProvider,
		ILogger<MainWindowViewModel> logger) {
		this.ConfigStore = configStore;
		this.StateStore = stateStore;
		this._stringProvider = stringProvider;
		this._logger = logger;

		// クリックカウンター初期化
		this.ClickCount = new BindableReactiveProperty<int>(0).AddTo(this.CompositeDisposable);
		this.IncrementCommand = new ReactiveCommand<Unit>().AddTo(this.CompositeDisposable);

		this.IncrementCommand.Subscribe(_ => {
			this.ClickCount.Value++;
			this._logger.LogInformation("IncrementCommand が実行されました。現在のカウント: {Count}", this.ClickCount.Value);
		}).AddTo(this.CompositeDisposable);

		// テキスト入力同期初期化
		this.InputText = new BindableReactiveProperty<string>(string.Empty).AddTo(this.CompositeDisposable);

		var initialEmpty = stringProvider.GetString("Main_EmptyInput");
		this.OutputText = this.InputText
			.Select(text => string.IsNullOrWhiteSpace(text)
				? this._stringProvider.GetString("Main_EmptyInput")
				: this._stringProvider.GetString("Main_EchoFormat", text))
			.ToBindableReactiveProperty(initialEmpty)
			.AddTo(this.CompositeDisposable);

		this._logger.LogInformation("MainWindowViewModel が初期化されました。");
	}
}