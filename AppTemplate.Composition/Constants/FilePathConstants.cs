namespace AppTemplate.Composition.Constants;

/// <summary>
/// アプリケーション内で使用されるファイル・ディレクトリパス定数。
/// </summary>
public static class FilePathConstants {
	private static readonly string DefaultBaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AppTemplate");
	private static string? _baseDirectory;

	/// <summary>
	/// アプリケーションデータのベースディレクトリ
	/// </summary>
	public static string BaseDirectory {
		get {
			return CurrentBaseDirectory;
		}
	}

	/// <summary>
	/// 設定ファイルの保存先パス
	/// </summary>
	public static string ConfigFilePath {
		get {
			return Path.Combine(CurrentBaseDirectory, "AppTemplate.config");
		}
	}

	/// <summary>
	/// 状態設定ファイルの保存先パス
	/// </summary>
	public static string StateFilePath {
		get {
			return Path.Combine(CurrentBaseDirectory, "AppTemplate.states");
		}
	}

	/// <summary>
	/// ログファイルの保存先ディレクトリパス
	/// </summary>
	public static string LogDirectoryPath {
		get {
			return Path.Combine(CurrentBaseDirectory, "log");
		}
	}

	/// <summary>
	/// ベースディレクトリを上書きします（テスト環境やポータブル起動時などに利用）。
	/// </summary>
	/// <param name="baseDirectory">上書きするベースディレクトリ</param>
	public static void OverrideBaseDirectory(string? baseDirectory) {
		if (string.IsNullOrWhiteSpace(baseDirectory)) {
			throw new ArgumentException("Base directory must not be empty or whitespace.", nameof(baseDirectory));
		}

		_baseDirectory = baseDirectory;
	}

	private static string CurrentBaseDirectory {
		get {
			return _baseDirectory ?? DefaultBaseDirectory;
		}
	}
}