using GenJsonConfig.Attributes;

namespace AppTemplate.Composition.Stores.Config.Model;

/// <summary>
/// アプリケーション全体の設定モデル
/// </summary>
[Inject(InjectServiceLifetime.Singleton)]
[GenerateJsonConfigDto]
public class AppConfigModel(LanguageConfigModel languageConfigModel, EnvironmentConfigModel environmentConfigModel) {
	/// <summary>
	/// 設定バージョン
	/// </summary>
	public int Version {
		get;
		set;
	} = 1;

	/// <summary>
	/// 言語設定
	/// </summary>
	public LanguageConfigModel LanguageConfig {
		get;
		set;
	} = languageConfigModel;

	/// <summary>
	/// 環境設定
	/// </summary>
	public EnvironmentConfigModel EnvironmentConfig {
		get;
		set;
	} = environmentConfigModel;
}