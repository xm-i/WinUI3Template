using GenJsonConfig.Attributes;

namespace AppTemplate.Composition.Stores.State.Model;

/// <summary>
/// アプリケーション全体の状態管理モデル
/// </summary>
[Inject(InjectServiceLifetime.Singleton)]
[GenerateJsonConfigDto]
public class AppStateModel(WindowStateModel windowStateModel) {
	/// <summary>
	/// ウィンドウ状態
	/// </summary>
	public WindowStateModel WindowState {
		get;
		set;
	} = windowStateModel;
}