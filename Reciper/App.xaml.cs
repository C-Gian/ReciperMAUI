namespace Reciper
{
    public partial class App : Application
    {
        public App(IFirebaseAuth auth)
        {
            InitializeComponent();

            MainPage = new AppShell();

            _ = Task.Run(async () =>
            {
                var ok = await auth.SignInAsync("g.culaon@gmail.com", "porcodio_99");
                System.Diagnostics.Debug.WriteLine($"Firebase login: {ok}");
            });
        }
    }
}