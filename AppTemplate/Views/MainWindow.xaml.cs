using AppTemplate.Composition.Interfaces;
using AppTemplate.Composition.Interfaces.Stores;
using AppTemplate.ViewModels;
using AutoDiAttributes;
using Microsoft.UI.Xaml;
using R3;

namespace AppTemplate.Views;

/// <summary>
/// メインウィンドウ
/// </summary>
[Inject(InjectServiceLifetime.Transient)]
public sealed partial class MainWindow : Window {
	private readonly CompositeDisposable _disposable = new();
	private readonly IStringProvider _stringProvider;
	private readonly IStateStore _stateStore;

	/// <summary>
	/// ウィンドウにバインドされた ViewModel
	/// </summary>
	public MainWindowViewModel ViewModel {
		get;
	}

	/// <summary>
	/// <see cref="MainWindow"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	/// <param name="viewModel">メインウィンドウ ViewModel</param>
	/// <param name="configStore">設定ストア</param>
	/// <param name="stateStore">状態ストア</param>
	/// <param name="stringProvider">ローカライズ文字列プロバイダー</param>
	public MainWindow(
		MainWindowViewModel viewModel,
		IConfigStore configStore,
		IStateStore stateStore,
		IStringProvider stringProvider) {
		this.ViewModel = viewModel;
		this._stringProvider = stringProvider;
		this._stateStore = stateStore;

		this.InitializeComponent();

		// ウィンドウタイトルのローカライズ設定（WinUI 3 では Window.Title に x:Uid が使えないため直接代入）
		this.Title = stringProvider.GetString("App_Title");

		// ウィンドウアイコンの設定
		var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "icon.ico");
		if (System.IO.File.Exists(iconPath)) {
			this.AppWindow.SetIcon(iconPath);
		}

		// ウィンドウサイズ復元
		var windowState = stateStore.State.WindowState;
		this.AppWindow.Resize(new Windows.Graphics.SizeInt32(
			(int)windowState.Width.Value,
			(int)windowState.Height.Value));

		// ウィンドウサイズ変更の記録
		this.AppWindow.Changed += (s, args) => {
			if (args.DidSizeChange) {
				windowState.Width.Value = this.AppWindow.Size.Width;
				windowState.Height.Value = this.AppWindow.Size.Height;
			}
		};

		// ViewModel のクローズ要求を購読
		viewModel.RequestClose.Subscribe(_ => this.Close()).AddTo(this._disposable);

		this.Closed += (s, e) => {
			this._disposable.Dispose();
			viewModel.Dispose();
		};
	}
}