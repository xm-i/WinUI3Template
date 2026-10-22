using AppTemplate.Composition.Stores.State.Model;

namespace AppTemplate.Composition.Interfaces.Stores;

/// <summary>
/// アプリケーション状態の永続化ストアインターフェース
/// </summary>
public interface IStateStore : IDisposableBase {
	/// <summary>
	/// 現在の状態モデル
	/// </summary>
	public AppStateModel State {
		get;
	}

	/// <summary>
	/// 状態をファイルから読み込みます。
	/// </summary>
	public void Load();

	/// <summary>
	/// 状態をファイルへ保存します。
	/// </summary>
	public void Save();
}