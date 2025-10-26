using Reciper.ViewModels;
using Reciper.Models;

namespace Reciper.Views;

public partial class RecipeDetailPage : ContentPage
{
    readonly RecipeDetailViewModel _vm;
    public RecipeDetailPage(RecipeDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
    public void Init(Recipe r) => _vm.Load(r);

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var page = Handler.MauiContext!.Services.GetRequiredService<RecipeEditPage>();
        ((RecipeEditViewModel)page.BindingContext).Load(_vm.Item!);
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_vm.Item is null) return;

        var ok = await DisplayAlert("Conferma", "Eliminare questa ricetta?", "Elimina", "Annulla");
        if (!ok) return;

        var repo = Handler.MauiContext!.Services.GetRequiredService<IRecipeRepository>();
        await repo.DeleteAsync(_vm.Item.Id!);

        await Shell.Current.Navigation.PopAsync();
    }
}