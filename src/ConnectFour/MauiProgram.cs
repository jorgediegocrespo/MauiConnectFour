using ConnectFour.Features;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace ConnectFour;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseSkiaSharp()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
			.Services
                .AddTransient<MainViewModel>()
                .AddTransient<MainPage>();

        return builder.Build();
	}
}
