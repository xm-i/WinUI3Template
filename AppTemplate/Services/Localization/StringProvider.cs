using AppTemplate.Composition.Interfaces;
using AutoDiAttributes;
using Microsoft.Windows.ApplicationModel.Resources;

namespace AppTemplate.Services.Localization;

/// <summary>
/// <see cref="ResourceManager"/> を使用する <see cref="IStringProvider"/> 実装。
/// Unpackaged環境でも言語設定（PrimaryLanguageOverride）が確実に反映されるように ResourceContext を明示的に使用します。
/// </summary>
[Inject(InjectServiceLifetime.Singleton, typeof(IStringProvider))]
public class StringProvider : IStringProvider {
	private readonly ResourceMap _resourceMap;
	private readonly ResourceContext _context;

	/// <summary>
	/// <see cref="StringProvider"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	public StringProvider() {
		var manager = new ResourceManager();
		this._resourceMap = manager.MainResourceMap.GetSubtree("Resources");
		this._context = manager.CreateResourceContext();

		// Unpackaged環境では、ResourceManager作成直後にPrimaryLanguageOverrideが反映されない場合があるため
		// 明示的にコンテキストの言語を上書きする
		var language = Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;
		if (!string.IsNullOrEmpty(language)) {
			this._context.QualifierValues["Language"] = language;
		}
	}

	/// <inheritdoc />
	public string GetString(string key) {
		if (string.IsNullOrEmpty(key)) {
			return string.Empty;
		}

		try {
			var candidate = this._resourceMap.GetValue(key, this._context);
			return candidate?.ValueAsString ?? key;
		} catch {
			return key;
		}
	}

	/// <inheritdoc />
	public string GetString(string key, params object?[] args) {
		var format = this.GetString(key);
		if (args == null || args.Length == 0) {
			return format;
		}
		try {
			return string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args);
		} catch (FormatException) {
			return format;
		}
	}
}