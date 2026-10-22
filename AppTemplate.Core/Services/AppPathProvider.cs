using AppTemplate.Composition.Constants;
using AppTemplate.Composition.Interfaces.Services;

namespace AppTemplate.Core.Services;

/// <summary>
/// アプリケーションの各パスを提供するサービス実装
/// </summary>
[Inject(InjectServiceLifetime.Singleton, typeof(IAppPathProvider))]
public class AppPathProvider : IAppPathProvider {
	/// <inheritdoc />
	public string BaseDirectory {
		get {
			return FilePathConstants.BaseDirectory;
		}
	}

	/// <inheritdoc />
	public string ConfigFilePath {
		get {
			return FilePathConstants.ConfigFilePath;
		}
	}

	/// <inheritdoc />
	public string StateFilePath {
		get {
			return FilePathConstants.StateFilePath;
		}
	}

	/// <inheritdoc />
	public string LogDirectoryPath {
		get {
			return FilePathConstants.LogDirectoryPath;
		}
	}
}