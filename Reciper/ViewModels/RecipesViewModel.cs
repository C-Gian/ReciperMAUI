using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reciper.Models;
using System.Collections.ObjectModel;

namespace Reciper.ViewModels;

public partial class RecipesViewModel : ObservableObject
{
    private readonly IRecipeRepository _repo;

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;

    public ObservableCollection<Recipe> Items { get; } = new();

    public RecipesViewModel(IRecipeRepository repo)
    {
        _repo = repo;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var list = await _repo.GetAllAsync();
            Items.Clear();
            foreach (var r in list) Items.Add(r);
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task RefreshAsync() => await LoadAsync();
}
