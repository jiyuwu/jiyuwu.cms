using Microsoft.Extensions.Logging;

namespace JIYUWU.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // 注册 LocalizationService
            builder.Services.AddSingleton<LocalizationService>();
            // 注册 MainPageViewModel
            builder.Services.AddTransient<ViewModel.MainPageViewModel>();

            // 注册 MainPage
            builder.Services.AddTransient<MainPage>(); // 确保这里的注册
            return builder.Build();
        }
    }
}
