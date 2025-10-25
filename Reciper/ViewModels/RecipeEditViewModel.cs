using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using Reciper.Models;
using Reciper.Services;

namespace Reciper.ViewModels;

public partial class RecipeEditViewModel : ObservableObject
{
    private readonly IRecipeRepository _repo;
    private readonly IImageService _imageSvc;
    private readonly IStorageService _storageSvc;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string guide = "";
    [ObservableProperty] private string tips = "";
    [ObservableProperty] private int prepMinutes;
    [ObservableProperty] private int servings = 1;
    [ObservableProperty] private string imageUrl = "";

    public RecipeEditViewModel(IRecipeRepository repo, IImageService img, IStorageService store)
    {
        _repo = repo;
        _imageSvc = img;
        _storageSvc = store;
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        var bytes = await _imageSvc.PickAndCompressJpegAsync();
        if (bytes is null) return;

        var fileName = $"{Guid.NewGuid():N}.jpg";
        var (storagePath, downloadUrl) = await _storageSvc.UploadImageAsync(bytes, fileName);
        ImageUrl = downloadUrl;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var recipe = new Recipe
        {
            Title = Title,
            Guide = Guide,
            Tips = Tips,
            PrepMinutes = PrepMinutes,
            Servings = Servings,
            ImagePath = ImageUrl
        };

        await _repo.CreateAsync(recipe);
        await Shell.Current.GoToAsync("..");
    }
}
