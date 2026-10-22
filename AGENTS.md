# AppTemplate AI Assistant Rules (AGENTS.md)

このファイルは、AIアシスタント（Agent）が本プロジェクトでコードを読み書きする際の**基本ルールとコンテキスト**を定義するものです。AIアシスタントはこのファイルの内容を優先して遵守します。

## 1. プロジェクトの概要と方針
- **プロジェクトの目的**: WinUI 3 + R3 による汎用スターターテンプレート（ボイラープレート）。クリーンなマルチプロジェクト構成、リアクティブMVVM、厳格なリソース破棄管理、DI自動登録、設定永続化基盤を提供します。
- **コミュニケーション**: AIアシスタントとのチャットおよびすべての回答（実装プラン等を含む）は**「日本語」**で行うこと。

## 2. 技術スタック
- **言語・フレームワーク**: C#, .NET 10 (`net10.0-windows10.0.26100.0`), WinUI 3 (Windows App SDK)
- **アーキテクチャ**: MVVMパターン
- **リアクティブ・状態管理**: R3 (`ReactiveProperty`, `ReactiveCommand`, `ObservableList`)
- **DI (依存性注入)**: `AutoDiAttributes` (Source Generator による自動登録) + `Microsoft.Extensions.DependencyInjection` + `CommunityToolkit.Mvvm.DependencyInjection.Ioc`
- **設定・状態ファイル永続化**: `GenJsonConfig` (Source Generator による DTO/コンバーター自動生成) + `System.Text.Json`
- **静的解析**: `AppTemplate.Analyzers` (`RequireAddToAnalyzer` による `AddTo` 漏れ検知)
- **ロギング**: `Serilog` (デバッグ出力 & ローリングファイルログ)
- **ユニットテスト**: `xUnit`, `Shouldly`, `Moq`

## 3. プロジェクト構成と責務
ソリューションは以下のマルチプロジェクト構成で責務が分離されています。コードの配置や参照時は各プロジェクトの責務を厳守してください。

### アプリケーション層
- **`AppTemplate/`** (メインアプリケーション / WinUI 3):
  - View (XAML) とそのコードビハインド、UI固有のライフサイクル管理、ウィンドウ制御サービス、多言語リソース（`Strings/`）を配置。
- **`AppTemplate.ViewModels/`** (ViewModel層):
  - アプリケーション全体の ViewModel（`MainWindowViewModel` 等）。View と Model の調停を R3 でリアクティブに行う。
- **`AppTemplate.Core/`** (ドメインモデル・サービス基盤):
  - ビジネスロジック、コアモデル、ドメインサービスを集約。
- **`AppTemplate.Composition/`** (共通契約・DI・システム共通設定):
  - アプリケーション全体の共通インターフェース（`Interfaces/`）、定数（`Constants/`）、設定モデル（`Stores/Config/Model/`）、状態モデル（`Stores/State/Model/`）。
- **`AppTemplate.Store/`** (永続化ストア):
  - 設定（Config）や状態（State）の JSON ファイル永続化処理（`ConfigStore`, `StateStore`）、JSON シリアライザーコンテキスト。
- **`AppTemplate.Common/`** (共通基盤):
  - プロジェクト横断で使用される基底クラス（`DisposableBase`, `ViewModelBase`, `ModelBase`, `ServiceBase`）、共通ヘルパー、拡張メソッド。
- **`AppTemplate.Core.Tests/`** (ユニットテスト):
  - `AppTemplate.Core` やストア等を対象とした xUnit 単体テスト。

### 自社製基盤ライブラリ・Source Generator (`lib/` 配下)
- **`lib/AutoDiAttributes/`**: `[Inject]` 属性による DI（依存性の注入）登録自動化 Source Generator。
- **`lib/GenJsonConfig/`**: `ReactiveProperty` や `ObservableList` に対応した JSON DTO / シリアライザー自動生成 Source Generator。
- **`lib/AppTemplate.Analyzers/`**: R3 の `AddTo(this.CompositeDisposable)` 漏れを静的解析で検出するカスタム Roslyn アナライザー (`RequireAddToAnalyzer`)。

---

## 4. コーディング規約と実装上のルール

### 4.1 コメント規約
- コメントは必ず「日本語」で記述すること。
- 新規作成・編集したクラス、メソッド、プロパティには必ずXMLドキュメンテーションコメント（`/// <summary>...`）を付与すること。

### 4.2 MVVMパターンの遵守
- View (XAML) と ViewModel の分離を徹底する。コードビハインド（.xaml.cs）への記述は最小限にし、純粋なUI制御（アニメーション、ウィンドウハンドル制御など）のみに留める。

### 4.3 R3の使用規約 (非常に重要)
- **リアクティブ設計**: 非同期処理、データバインディング、コマンドの実装には `R3` を活用する。
- **コード生成属性の禁止**: `[ObservableProperty]` や `[RelayCommand]` といった CommunityToolkit のジェネレーター属性は使用せず、`ReactiveProperty` や `ReactiveCommand` 等でリアクティブに設計する。
- **AddTo の徹底**: 購読の解除漏れを防ぐため、すべての `ReactiveProperty`、`ReactiveCommand`、`Subscribe`、`SubscribeAwait` などの `IDisposable` オブジェクトは必ず `.AddTo(this.CompositeDisposable)` を呼び出すこと。
- **非同期購読には `SubscribeAwait` を使用**: `Subscribe(async _ => ...)` のような `async void` 相当の書き方は禁止。非同期処理が伴う購読は必ず `SubscribeAwait` を使用し、必要に応じて `AwaitOperation` を明示的に指定すること。

### 4.4 Disposeルール (非常に重要)
- すべての ViewModel、Model、Service は `DisposableBase` を継承する基底クラス（`ViewModelBase` / `ModelBase` / `ServiceBase`）を継承すること。
- `IDisposable` を返す全ての処理は、破棄責任者を明確にし、通常は `.AddTo(this.CompositeDisposable)` すること。
- `Dispose(bool)` のオーバーライドで独自リソースを解放する場合は、必ず `base.Dispose(disposing)` を呼ぶこと。

### 4.5 UIデータバインディング規約
- **`{x:Bind}` を優先**: XAML のデータバインディングでは `{Binding}` ではなく型安全かつ高速な `{x:Bind}` を原則使用すること。
- **コレクションのバインディング**: `ObservableList<T>` 等を WinUI 3 の UI（ListView, ItemsRepeater等）にバインドする場合は、ViewModel 側で `.ToNotifyCollectionChanged(SynchronizationContextCollectionEventDispatcher.Current)` を通して UI スレッド同期されたビューリストとして公開すること。

### 4.6 設定・状態管理 (`GenJsonConfig`)
- アプリケーション設定および状態管理には `GenJsonConfig` を利用する。
- 保存対象のモデルクラスには `[GenerateJsonConfigDto]` を付与し、DTO と相互変換メソッドを自動生成させる。
- `System.Text.Json` の Source Generator（`JsonSerializerContext`）に生成された DTO（`*ForJson`）を登録する。

### 4.7 DI (依存性注入) と起動シーケンス
- クラスには `[Inject(InjectServiceLifetime.Singleton)]` や `[Inject(InjectServiceLifetime.Transient, typeof(IInterface))]` を付与する。
- `App.xaml.cs` の `BuildConfigureServices()` で、各プロジェクトの `DIRegistration.AddGeneratedServices(services)` を呼び出して一括登録する。
- `Ioc.Default` にサービスプロバイダーを設定し、サービスロケーションが必要な箇所でも一貫して利用できる構成にする。

### 4.8 国際化 (i18n)
- UI上の文字列（ラベル、ボタン、ツールチップ等）は直接ハードコードせず、必ず `Strings/ja/Resources.resw` および `Strings/en/Resources.resw` に定義すること。
- XAML要素のローカライズには `x:Uid` を使用すること（例: `MyButton.Content`）。
- **Window の Title について**: WinUI 3 の `Window` 要素は `x:Uid` による `Title` 設定をサポートしていません（ビルドエラーになります）。ウィンドウタイトルはコードビハインドで `IStringProvider` を使用して `this.Title` に代入してください。
- ViewModel やサービス内でローカライズ文字列が必要な場合は、`IStringProvider` をコンストラクタで注入して使用すること。

### 4.9 作業完了前の検証 (必須)
作業を終了する前に、必ず以下を実行し、エラー・警告・テスト失敗・フォーマット違反がすべてゼロであることを確認すること：
1. `dotnet build`
2. `dotnet test`
3. `dotnet format --verify-no-changes`
