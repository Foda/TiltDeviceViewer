using ReactiveUI.Maui;
using TiltViewer.ViewModels;

namespace TiltViewer.View;

public partial class TiltHydrometerView : ReactiveContentView<TiltHydrometerViewModel>
{
	public TiltHydrometerView()
	{
		InitializeComponent();
	}

    private async void SetBatchName_Clicked(object sender, EventArgs e)
    {
		string result = await this.Window.Page.DisplayPromptAsync("Set Batch Name", "Enter a name for the associated batch:");
		if (result != null)
		{
			this.ViewModel.SetBatchName(result);
		}
    }
}