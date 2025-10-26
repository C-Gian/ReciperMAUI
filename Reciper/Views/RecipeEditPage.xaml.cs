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

    //private void OnForceImage(object sender, EventArgs e)
    //{
    //    ((Reciper.ViewModels.RecipeEditViewModel)BindingContext).ImageUrl =
    //        "https://via.placeholder.com/600x400.jpg";
    //    System.Diagnostics.Debug.WriteLine("Forced ImageUrl");
    //}

    //private async void OnPickClicked(object sender, EventArgs e)
    //{
    //    var vm = (Reciper.ViewModels.RecipeEditViewModel)BindingContext;
    //    System.Diagnostics.Debug.WriteLine("BTN CLICK");
    //    System.Diagnostics.Debug.WriteLine("CMD null? " + (vm.PickImageCommand == null));
    //    if (vm.PickImageCommand != null)
    //        await vm.PickImageCommand.ExecuteAsync(null);
    //}
}
