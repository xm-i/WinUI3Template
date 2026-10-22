# WinUI 3 + R3 汎用スターターテンプレート (AppTemplate)

.NET 10 (`net10.0-windows10.0.26100.0`) および WinUI 3 (Windows App SDK)、R3 リアクティブプログラミングモデルを採用した汎用デスクトップアプリケーション開発スターターテンプレート（ボイラープレート）です。

厳格なリソース破棄管理、Source Generator による DI 自動登録・JSON 設定シリアライズ、多言語対応 (i18n)、Roslyn アナライザーによる `AddTo` 漏れ検知を備えています。

---

## 主な特徴・アーキテクチャ

1. **WinUI 3 + R3 リアクティブ MVVM**
   - R3 の `ReactiveProperty<T>`, `BindableReactiveProperty<T>`, `ReactiveCommand<T>` を活用。
   - View では `{Binding}` ではなく **`{x:Bind}`** を徹底。
   - `CommunityToolkit.Mvvm` の `[ObservableProperty]` などのソースジェネレータ属性は使用せず、R3 の明示的かつ堅牢なプロパティ管理を実施。
2. **厳格な破棄管理 (Lifecycle & Disposal)**
   - 全ての ViewModel / Model / Service は `DisposableBase` を継承。
   - すべてのイベント購読・プロパティ生成に `.AddTo(this.CompositeDisposable)` を適用。
   - コレクション同期ビューには要素自動破棄つきの `CreateViewWithDisposal` / `ToNotifyCollectionChangedWithDisposal` を提供。
3. **カスタム Roslyn アナライザー (`lib/AppTemplate.Analyzers`)**
   - `AddTo` や破棄処理が漏れている `IDisposable`（`Subscribe`, `ReactiveProperty` 等）をビルド時に自動検知（診断コード: `AT0001`）。
4. **Source Generator による DI 自動登録 (`lib/AutoDiAttributes`)**
   - `[Inject(InjectServiceLifetime.Singleton / Transient / Scoped)]` 属性を付与するだけで、コンパイル時に DI コンテナ登録コードを自動生成。
   - `App.xaml.cs` の `DIRegistration.AddGeneratedServices(services)` で一括登録。
5. **JSON 設定・状態永続化 (`lib/GenJsonConfig`)**
   - `[JsonConfig]` 属性による高速・リフレクションフリーな JSON 永続化基盤。
   - `ConfigStore`（アプリケーション設定）および `StateStore`（ウィンドウ位置・サイズ等）を標準搭載。
6. **多言語対応 (i18n)**
   - WinUI 3 の `x:Uid`（XAML 側）と `IStringProvider`（ロジック・ViewModel 側）による日・英完全ローカライズ。
7. **構造化ロギング (Serilog)**
   - ファイルローリングログ（`%LocalAppData%/AppTemplate/logs/`）およびデバッグ出力（Visual Studio 出力ウィンドウ）に初期化済み。

---

## ソリューション構成

```text
AppTemplate.slnx
├── lib/
│   ├── AutoDiAttributes/        # [git submodule] DI 自動登録 Generator
│   ├── GenJsonConfig/           # [git submodule] JSON 設定・状態永続化 Generator
│   └── AppTemplate.Analyzers/   # R3 AddTo 漏れ検知 Roslyn アナライザー (AT0001)
├── AppTemplate/                 # WinUI 3 メインアプリケーション (Views, App.xaml, MainWindow, ウィンドウ制御)
├── AppTemplate.ViewModels/      # ViewModel 層 (MainWindowViewModel 等)
├── AppTemplate.Core/            # ドメインモデル・サービス基盤 (AppPathProvider 等)
├── AppTemplate.Composition/     # DI 共通契約、インターフェース、設定モデル、定数
├── AppTemplate.Common/          # DisposableBase, ViewModelBase, ModelBase, ServiceBase, R3Ex, Collections
├── AppTemplate.Store/           # GenJsonConfig を用いた設定ファイル永続化基盤 (ConfigStore, StateStore)
├── AppTemplate.Core.Tests/      # xUnit 単体テストプロジェクト (ConfigStore, StateStore, DisposableBase 等)
├── .editorconfig                # コーディング規約・フォーマット定義
├── Directory.Build.props        # ソリューション共通ビルド設定 (全PJへアナライザー適用等)
├── Directory.Build.targets      # パッケージバージョン一元管理
├── AGENTS.md                    # テンプレート用コーディング規約・設計指針
└── .github/
    └── copilot-instructions.md  # GitHub Copilot 用規約参照設定
```

---

## 開発環境とビルド手順

### 必要要件
- **Windows 11 / Windows 10 (Version 1809 以上)**
- **.NET 10 SDK**
- **Visual Studio 2022 / 2026** (「.NET デスクトップ開発」および「Windows アプリケーション開発」ワークロード)

### クローンと submodule の初期化
```bash
git clone <repository-url> MyApp
cd MyApp
git submodule update --init --recursive
```

### ソリューションのビルドとテスト
```bash
# x64 ビルド
dotnet build AppTemplate.slnx -p:Platform=x64

# ARM64 ビルド
dotnet build AppTemplate.slnx -p:Platform=ARM64

# 単体テストの実行
dotnet test AppTemplate.slnx
```

---

## テンプレートのカスタマイズ（名前空間のリネーム）

本テンプレートのデフォルト名前空間およびプロジェクトプレフィックスは `AppTemplate` となっています。新規アプリケーションを作成する際は、以下の手順で一括置換してください：

1. ディレクトリ名・プロジェクトファイル名（`AppTemplate.*`）を `YourApp.*` に変更
2. `AppTemplate.slnx` 内のプロジェクトパス・名前を更新
3. ソリューション内の全ファイルで `AppTemplate` を `YourApp` に一括置換
4. `Package.appxmanifest` および `Package.Dev.appxmanifest` の `<Identity Name="..." />` や表示名を更新

---

## コーディング規約

詳細なコーディング規約、R3 設計原則、破棄ルールについては [AGENTS.md](AGENTS.md) を参照してください。
