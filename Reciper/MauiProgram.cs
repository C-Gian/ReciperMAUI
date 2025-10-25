using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Reciper.Models;

namespace Reciper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<IFirebaseAuth, FirebaseAuth>();
            builder.Services.AddSingleton<IRecipeRepository, FirestoreRecipeRepository>();
            builder.Services.AddTransient<Reciper.Views.RecipesPage>();
            builder.Services.AddTransient<Reciper.ViewModels.RecipesViewModel>();

            return builder.Build();
        }
    }
}
