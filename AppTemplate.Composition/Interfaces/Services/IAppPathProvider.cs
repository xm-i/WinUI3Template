namespace AppTemplate.Composition.Interfaces.Services;

/// <summary>
/// アプリケーションのファイルパスを提供するインターフェース。
/// テスト環境と本番環境でパスを切り替えるためにDI経由で注入します。
/// </summary>
public interface IAppPathProvider {
	/// <summary>
	/// アプリケーションデータのベースディレクトリ
	/// </summary>
	public string BaseDirectory {
		get;
	}

	/// <summary>
	/// 設定ファイルのパス
	/// </summary>
	public string ConfigFilePath {
		get;
	}

	/// <summary>
	/// 状態設定ファイルのパス
	/// </summary>
	public string StateFilePath {
		get;
	}

	/// <summary>
	/// ログディレクトリのパス
	/// </summary>
	public string LogDirectoryPath {
		get;
	}
}