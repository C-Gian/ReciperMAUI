using Reciper.ViewModels;

namespace Reciper.Views;

public partial class RecipesPage : ContentPage
{
    private readonly RecipesViewModel _vm;
    public RecipesPage(RecipesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Items.Count == 0) await _vm.LoadAsync();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.FirstOrDefault() is not Reciper.Models.Recipe r) return;
        var page = Handler.MauiContext!.Services.GetRequiredService<Reciper.Views.RecipeDetailPage>();
        page.Init(r);
        await Shell.Current.Navigation.PushAsync(page);
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var page = Handler.MauiContext!.Services.GetRequiredService<Reciper.Views.RecipeEditPage>();
        await Shell.Current.Navigation.PushAsync(page);
    }

}
