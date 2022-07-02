using ConnectFour.Features;
using System.Globalization;

namespace ConnectFour;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		MainPage = serviceProvider.GetService<MainPage>();
	}

    protected override void OnStart()
    {
        base.OnStart();
        SetCulture(CultureInfo.CurrentCulture);
    }

    private void SetCulture(CultureInfo cultureInfo)
    {
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}
