namespace AppTemplate.Composition.Enum;

/// <summary>
/// オブジェクトの Dispose 状態を表します。
/// </summary>
public enum DisposeState {
	/// <summary>
	/// Dispose されていない
	/// </summary>
	NotDisposed,

	/// <summary>
	/// Dispose 処理中
	/// </summary>
	Disposing,

	/// <summary>
	/// Dispose 完了済み
	/// </summary>
	Disposed
}