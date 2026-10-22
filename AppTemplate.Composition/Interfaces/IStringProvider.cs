namespace AppTemplate.Composition.Interfaces;

/// <summary>
/// ローカライズ済み文字列リソースへのアクセサ。
/// ViewModel / Service など UI 非依存層から Resources.resw のキーを解決するために利用します。
/// </summary>
public interface IStringProvider {
	/// <summary>
	/// 指定したリソースキーに対応するローカライズ文字列を取得します。
	/// 見つからない場合は <paramref name="key"/> をそのまま返します。
	/// </summary>
	/// <param name="key">resw のキー（例: "App_Title"）</param>
	/// <returns>ローカライズ文字列</returns>
	public string GetString(string key);

	/// <summary>
	/// フォーマット引数を適用したローカライズ文字列を取得します。
	/// </summary>
	/// <param name="key">resw のキー</param>
	/// <param name="args">書式設定引数</param>
	/// <returns>書式適用済みローカライズ文字列</returns>
	public string GetString(string key, params object?[] args);
}