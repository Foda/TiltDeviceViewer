using TiltViewer.ViewModels;

namespace TiltViewer;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		ShellMain.BindingContext = new MainPageViewModel();
    }
}
