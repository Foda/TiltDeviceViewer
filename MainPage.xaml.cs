using ReactiveUI.Maui;
using TiltViewer.Beacon;
using TiltViewer.Platforms.Windows;
using TiltViewer.View;
using TiltViewer.ViewModels;
namespace TiltViewer;

public partial class MainPage : ReactiveContentPage<MainPageViewModel>
{
    public MainPage()
	{
		InitializeComponent();
    }
}