using Reciper.ViewModels;

namespace Reciper.Views;

public partial class RecipeEditPage : ContentPage
{
    public RecipeEditPage(RecipeEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        System.Diagnostics.Debug.WriteLine("BC=" + BindingContext?.GetType().FullName);
    }

    private void OnForceImage(object sender, EventArgs e)
    {
        ((Reciper.ViewModels.RecipeEditViewModel)BindingContext).ImageUrl =
            "https://via.placeholder.com/600x400.jpg";
        System.Diagnostics.Debug.WriteLine("Forced ImageUrl");
    }

}
