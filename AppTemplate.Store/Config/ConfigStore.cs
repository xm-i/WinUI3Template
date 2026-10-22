using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AppTemplate.Common.Base;
using AppTemplate.Composition.Interfaces;
using AppTemplate.Composition.Interfaces.Services;
using AppTemplate.Composition.Interfaces.Stores;
using AppTemplate.Composition.Stores.Config.Model;
using AppTemplate.Store.SerializerContext;
using GenJsonConfig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppTemplate.Store.Config;

/// <summary>
/// アプリケーション設定の永続化ストア実装
/// </summary>
[Inject(InjectServiceLifetime.Singleton, typeof(IConfigStore))]
public class ConfigStore : DisposableBase, IConfigStore {
	private readonly ILogger<ConfigStore> _logger;
	private readonly IAppPathProvider _pathProvider;
	private readonly IStringProvider? _stringProvider;

	/// <summary>
	/// サービスプロバイダー
	/// </summary>
	public IServiceProvider ScopedService {
		get;
	}

	/// <summary>
	/// 現在の設定モデル
	/// </summary>
	public AppConfigModel Config {
		get;
		private set;
	}

	/// <summary>
	/// 設定ファイルパス
	/// </summary>
	protected virtual string ConfigFilePath {
		get {
			return this._pathProvider.ConfigFilePath;
		}
	}

	/// <summary>
	/// <see cref="ConfigStore"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	/// <param name="service">サービスプロバイダー</param>
	/// <param name="pathProvider">アプリケーションパスプロバイダー</param>
	/// <param name="stringProvider">文字列プロバイダー（省略可能）</param>
	public ConfigStore(IServiceProvider service, IAppPathProvider pathProvider, IStringProvider? stringProvider = null) {
		this.ScopedService = service;
		this._pathProvider = pathProvider;
		this._stringProvider = stringProvider ?? service.GetService<IStringProvider>();
		this._logger = service.GetRequiredService<ILogger<ConfigStore>>();
		this.Load();
	}

	private JsonSerializerOptions? _jsonSerializerOptions;

	/// <summary>
	/// JSONシリアライズ設定
	/// </summary>
	public JsonSerializerOptions JsonSerializerOptions {
		get {
			return this._jsonSerializerOptions ??= new JsonSerializerOptions() {
				WriteIndented = true,
				TypeInfoResolver = ConfigJsonSerializerContext.Default.WithAddedModifier(ForJsonConverterRegistry.ApplyPolymorphism)
			};
		}
	}

	/// <summary>
	/// 保存済み設定を読み込みます。
	/// </summary>
	[MemberNotNull(nameof(Config))]
	public void Load() {
		if (this.Config != null) {
			this._logger.LogWarning("アプリケーション設定は既に読み込まれています。インスタンスの再生成はスキップされました。");
			return;
		}

		var scope = this.ScopedService.CreateScope().AddTo(this.CompositeDisposable);
		try {
			if (File.Exists(this.ConfigFilePath)) {
				try {
					var json = File.ReadAllText(this.ConfigFilePath);
					var loaded = JsonSerializer.Deserialize<AppConfigModelForJson>(json, this.JsonSerializerOptions);
					if (loaded != null) {
						this.Config = AppConfigModelForJson.CreateModel(loaded, scope.ServiceProvider);
						this._logger.LogInformation("アプリケーション設定の読み込みに成功しました");
						return;
					}
				} catch (Exception e) {
					this._logger.LogError(e, "アプリケーション設定ファイルのJSON解析に失敗しました: {FilePath}", this.ConfigFilePath);
				}
			}
		} catch (Exception ex) {
			this._logger.LogError(ex, "アプリケーション設定読み込みのスコープ生成に失敗しました");
		}

		// デフォルト設定を作成
		this.Config = scope.ServiceProvider.GetRequiredService<AppConfigModel>();
		this._logger.LogWarning("デフォルトのアプリケーション設定を使用します");
	}

	/// <summary>
	/// 現在の設定をファイルへ保存します。
	/// </summary>
	public void Save() {
		try {
			var directory = Path.GetDirectoryName(this.ConfigFilePath);
			if (!string.IsNullOrEmpty(directory)) {
				Directory.CreateDirectory(directory);
			}

			var jsonDto = AppConfigModelForJson.CreateJson(this.Config);
			var json = JsonSerializer.Serialize(jsonDto, this.JsonSerializerOptions);
			File.WriteAllText(this.ConfigFilePath, json);
			this._logger.LogInformation("アプリケーション設定を保存しました: {FilePath}", this.ConfigFilePath);
		} catch (Exception e) {
			this._logger.LogError(e, "アプリケーション設定ファイルの保存に失敗しました: {FilePath}", this.ConfigFilePath);
		}
	}
}