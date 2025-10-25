using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reciper.Models;
using Reciper.Services;
using System;

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

    // comando sonda
    [RelayCommand]
    private void Test() => System.Diagnostics.Debug.WriteLine("TEST CMD");

    [RelayCommand]
    private async Task PickImageAsync()
    {
        System.Diagnostics.Debug.WriteLine("PickImageAsync start");
        try
        {
            var bytes = await _imageSvc.PickAndCompressJpegAsync();
            System.Diagnostics.Debug.WriteLine("PickImageAsync bytes: " + (bytes?.Length ?? 0));
            if (bytes is null) return;

            var fileName = $"{Guid.NewGuid():N}.jpg";
            var (_, downloadUrl) = await _storageSvc.UploadImageAsync(bytes, fileName);
            ImageUrl = downloadUrl;
            System.Diagnostics.Debug.WriteLine("PickImageAsync done: " + ImageUrl);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("PickImageAsync ERR: " + ex);
            await CommunityToolkit.Maui.Alerts.Toast.Make("Errore selezione immagine").Show();
        }
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
