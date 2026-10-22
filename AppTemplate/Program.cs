using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace AppTemplate;

/// <summary>
/// アプリケーションのエントリポイント
/// </summary>
public static class Program {
	[System.Runtime.InteropServices.DllImport("Microsoft.ui.xaml.dll")]
	private static extern void XamlCheckProcessRequirements();

	/// <summary>
	/// アプリケーションのメインエントリポイント
	/// </summary>
	/// <param name="args">コマンドライン引数</param>
	[STAThread]
	public static void Main(string[] args) {
		XamlCheckProcessRequirements();

		Application.Start(static (p) => {
			var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
			SynchronizationContext.SetSynchronizationContext(context);
			_ = new App();
		});
	}
}