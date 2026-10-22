# GitHub Copilot Instructions for AppTemplate

このリポジトリにおけるコーディング規約、アーキテクチャ、および作業手順は [AGENTS.md](../AGENTS.md) に定義されています。
コード生成、編集、レビューを行う際は、AGENTS.md に記載されているルールを必ず遵守してください。

## 主な遵守事項
1. **言語・フレームワーク**: C# (.NET 10), WinUI 3 (Windows App SDK)
2. **リアクティブ・状態管理 (R3)**:
   - 非同期処理・コマンド・データバインディングには `R3` を使用する（`ReactiveProperty`, `ReactiveCommand` 等）。
   - すべての購読・ReactivePropertyは必ず `AddTo(this.CompositeDisposable)` すること。
   - 非同期購読には必ず `SubscribeAwait` を使用すること（`Subscribe(async ...)` は禁止）。
   - `[ObservableProperty]` などのコード生成属性は禁止。
3. **MVVM & Dispose規約**:
   - ViewModel/Model/Service は `DisposableBase` 派生クラス（`ViewModelBase`, `ModelBase`, `ServiceBase`）を継承すること。
   - View (XAML) では `{Binding}` ではなく `{x:Bind}` を優先すること。
   - `ObservableCollections` をバインドする際は `.ToNotifyCollectionChanged(SynchronizationContextCollectionEventDispatcher.Current)` でUIスレッドと同期すること。
4. **設定管理 (GenJsonConfig)**:
   - 設定・状態モデルは `[GenerateJsonConfigDto]` と `GenJsonConfig` を利用して永続化する。
5. **国際化 (i18n)**:
   - UI文字列はハードコードせず `Strings/ja/Resources.resw` および `Strings/en/Resources.resw` に定義すること。
   - XAML要素には `x:Uid` を使用すること。
   - ロジック内では `IStringProvider` を使用すること。
6. **作業完了前の検証**:
   - 作業完了前に `dotnet build`、`dotnet test`、および `dotnet format` を実行し、エラー・警告ゼロを確認すること。
