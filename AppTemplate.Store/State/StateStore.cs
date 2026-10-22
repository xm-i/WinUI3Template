using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AppTemplate.Common.Base;
using AppTemplate.Composition.Interfaces;
using AppTemplate.Composition.Interfaces.Services;
using AppTemplate.Composition.Interfaces.Stores;
using AppTemplate.Composition.Stores.State.Model;
using AppTemplate.Store.SerializerContext;
using GenJsonConfig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppTemplate.Store.State;

/// <summary>
/// アプリケーション状態の永続化ストア実装
/// </summary>
[Inject(InjectServiceLifetime.Singleton, typeof(IStateStore))]
public class StateStore : DisposableBase, IStateStore {
	private readonly ILogger<StateStore> _logger;
	private readonly IAppPathProvider _pathProvider;
	private readonly IStringProvider? _stringProvider;

	/// <summary>
	/// サービスプロバイダー
	/// </summary>
	public IServiceProvider ScopedService {
		get;
	}

	/// <summary>
	/// 現在の状態モデル
	/// </summary>
	public AppStateModel State {
		get;
		private set;
	}

	/// <summary>
	/// 状態ファイルパス
	/// </summary>
	protected virtual string StateFilePath {
		get {
			return this._pathProvider.StateFilePath;
		}
	}

	/// <summary>
	/// <see cref="StateStore"/> クラスの新しいインスタンスを初期化します。
	/// </summary>
	/// <param name="service">サービスプロバイダー</param>
	/// <param name="pathProvider">アプリケーションパスプロバイダー</param>
	/// <param name="stringProvider">文字列プロバイダー（省略可能）</param>
	public StateStore(IServiceProvider service, IAppPathProvider pathProvider, IStringProvider? stringProvider = null) {
		this.ScopedService = service;
		this._pathProvider = pathProvider;
		this._stringProvider = stringProvider ?? service.GetService<IStringProvider>();
		this._logger = service.GetRequiredService<ILogger<StateStore>>();
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
				TypeInfoResolver = StateJsonSerializerContext.Default.WithAddedModifier(ForJsonConverterRegistry.ApplyPolymorphism)
			};
		}
	}

	/// <summary>
	/// 保存済み状態を読み込みます。
	/// </summary>
	[MemberNotNull(nameof(State))]
	public void Load() {
		if (this.State != null) {
			this._logger.LogWarning("アプリケーション状態は既に読み込まれています。インスタンスの再生成はスキップされました。");
			return;
		}

		var scope = this.ScopedService.CreateScope().AddTo(this.CompositeDisposable);
		try {
			if (File.Exists(this.StateFilePath)) {
				try {
					var json = File.ReadAllText(this.StateFilePath);
					var loaded = JsonSerializer.Deserialize<AppStateModelForJson>(json, this.JsonSerializerOptions);
					if (loaded != null) {
						this.State = AppStateModelForJson.CreateModel(loaded, scope.ServiceProvider);
						this._logger.LogInformation("状態設定の読み込みに成功しました");
						return;
					}
				} catch (Exception e) {
					this._logger.LogError(e, "状態設定ファイルのJSON解析に失敗しました: {FilePath}", this.StateFilePath);
				}
			}
		} catch (Exception ex) {
			this._logger.LogError(ex, "状態設定読み込みのスコープ生成に失敗しました");
		}

		// デフォルト状態を作成
		this.State = scope.ServiceProvider.GetRequiredService<AppStateModel>();
		this._logger.LogWarning("デフォルトの状態設定を使用します");
	}

	/// <summary>
	/// 現在の状態をファイルへ保存します。
	/// </summary>
	public void Save() {
		try {
			var directory = Path.GetDirectoryName(this.StateFilePath);
			if (!string.IsNullOrEmpty(directory)) {
				Directory.CreateDirectory(directory);
			}

			var jsonDto = AppStateModelForJson.CreateJson(this.State);
			var json = JsonSerializer.Serialize(jsonDto, this.JsonSerializerOptions);
			File.WriteAllText(this.StateFilePath, json);
			this._logger.LogInformation("状態設定を保存しました: {FilePath}", this.StateFilePath);
		} catch (Exception e) {
			this._logger.LogError(e, "状態設定ファイルの保存に失敗しました: {FilePath}", this.StateFilePath);
		}
	}
}