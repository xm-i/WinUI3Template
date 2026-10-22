using AppTemplate.Composition.Stores.Config.Model;

namespace AppTemplate.Composition.Interfaces.Stores;

/// <summary>
/// アプリケーション設定の永続化ストアインターフェース
/// </summary>
public interface IConfigStore : IDisposableBase {
	/// <summary>
	/// 現在の設定モデル
	/// </summary>
	public AppConfigModel Config {
		get;
	}

	/// <summary>
	/// 設定をファイルから読み込みます。
	/// </summary>
	public void Load();

	/// <summary>
	/// 設定をファイルへ保存します。
	/// </summary>
	public void Save();
}