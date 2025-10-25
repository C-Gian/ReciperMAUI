using CommunityToolkit.Mvvm.ComponentModel;
using Reciper.Models;

namespace Reciper.ViewModels;

public partial class RecipeDetailViewModel : ObservableObject
{
    [ObservableProperty] private Recipe? item;
    public void Load(Recipe r) => Item = r;
}