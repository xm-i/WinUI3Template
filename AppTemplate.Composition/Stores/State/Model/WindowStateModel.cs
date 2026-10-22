using GenJsonConfig.Attributes;

namespace AppTemplate.Composition.Stores.State.Model;

/// <summary>
/// ウィンドウの表示状態モデル
/// </summary>
[Inject(InjectServiceLifetime.Singleton)]
[GenerateJsonConfigDto]
public class WindowStateModel {
	/// <summary>
	/// ウィンドウ幅
	/// </summary>
	public ReactiveProperty<double> Width {
		get;
	} = new(1000);

	/// <summary>
	/// ウィンドウ高さ
	/// </summary>
	public ReactiveProperty<double> Height {
		get;
	} = new(700);

	/// <summary>
	/// ウィンドウが最大化されているかどうか
	/// </summary>
	public ReactiveProperty<bool> IsMaximized {
		get;
	} = new(false);
}