using AppTemplate.Composition.Enum;
using GenJsonConfig.Attributes;

namespace AppTemplate.Composition.Stores.Config.Model;

/// <summary>
/// アプリケーションの環境設定モデル
/// </summary>
[Inject(InjectServiceLifetime.Singleton)]
[GenerateJsonConfigDto]
public class EnvironmentConfigModel {
	/// <summary>
	/// カラーテーマ設定
	/// </summary>
	public ReactiveProperty<AppTheme> Theme {
		get;
	} = new(AppTheme.Default);
}