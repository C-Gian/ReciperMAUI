using Reciper.Models;

namespace Reciper
{
    public partial class App : Application
    {
        public App(IFirebaseAuth auth, IRecipeRepository repo)
        {
            InitializeComponent();

            MainPage = new AppShell();

            _ = Task.Run(async () =>
            {
                var ok = await auth.SignInAsync(Reciper.Utils.DevSecrets.Email, Reciper.Utils.DevSecrets.Password);
                System.Diagnostics.Debug.WriteLine($"Firebase login: {ok}");

                if (!ok) return;

                // Crea ricetta di test
                var newRecipe = new Recipe
                {
                    Title = "Carbonara di test",
                    Guide = "Mescola uova, guanciale e pecorino.",
                    Tips = "Non aggiungere panna.",
                    PrepMinutes = 20,
                    Servings = 2,
                    Ingredients = new() { "Uova", "Guanciale", "Pecorino", "Pasta" },
                    Tags = new() { "Italiana", "Pasta" }
                };

                var id = await repo.CreateAsync(newRecipe);
                System.Diagnostics.Debug.WriteLine($"Creata ricetta ID: {id}");

                // Lettura di tutte le ricette
                var all = await repo.GetAllAsync();
                System.Diagnostics.Debug.WriteLine($"Tot ricette: {all.Count}");
            });
        }
    }
}