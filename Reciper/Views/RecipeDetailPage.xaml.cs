using Reciper.ViewModels;
using Reciper.Models;

namespace Reciper.Views;

public partial class RecipeDetailPage : ContentPage
{
    readonly RecipeDetailViewModel _vm;
    readonly IRecipeRepository _repo;

    public RecipeDetailPage(RecipeDetailViewModel vm, IRecipeRepository repo)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _repo = repo;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var id = _vm.Item?.Id;
        if (string.IsNullOrEmpty(id)) return;

        var fresh = await _repo.GetAsync(id);
        if (fresh != null) _vm.Load(fresh);
    }

    public void Init(Recipe r) => _vm.Load(r);

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var page = Handler.MauiContext!.Services.GetRequiredService<RecipeEditPage>();
        var vmEdit = (Reciper.ViewModels.RecipeEditViewModel)page.BindingContext;
        vmEdit.Load(_vm.Item!);
        await Shell.Current.Navigation.PushAsync(page);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_vm.Item is null) return;

        var ok = await DisplayAlert("Conferma", "Eliminare questa ricetta?", "Elimina", "Annulla");
        if (!ok) return;

        var repo = Handler.MauiContext!.Services.GetRequiredService<IRecipeRepository>();
        var storage = Handler.MauiContext!.Services.GetRequiredService<IStorageService>();

        await repo.DeleteAsync(_vm.Item.Id!);
        if (!string.IsNullOrWhiteSpace(_vm.Item!.StoragePath))  await storage.DeleteAsync(_vm.Item.StoragePath!);

        await Shell.Current.Navigation.PopAsync();
    }
}