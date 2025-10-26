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

    [ObservableProperty] private string? id;
    [ObservableProperty] private string title = "";
    [ObservableProperty] private string guide = "";
    [ObservableProperty] private string tips = "";
    [ObservableProperty] private int prepMinutes;
    [ObservableProperty] private int servings = 1;
    [ObservableProperty] private string imageUrl = "";
    [ObservableProperty] private string storagePath = "";
    [ObservableProperty] private string tagsCsv = ""; 


    private string? _originalStoragePath;
    private bool _imageChanged;

    public RecipeEditViewModel(IRecipeRepository repo, IImageService img, IStorageService store)
    {
        _repo = repo;
        _imageSvc = img;
        _storageSvc = store;
    }

    [RelayCommand]
    private async Task PickImage()
    {
        System.Diagnostics.Debug.WriteLine("PickImage start");
        try
        {
            var bytes = await _imageSvc.PickAndCompressJpegAsync();
            System.Diagnostics.Debug.WriteLine("PickImage bytes: " + (bytes?.Length ?? 0));
            if (bytes is null) return;

            var fileName = $"{Guid.NewGuid():N}.jpg";
            var (sp, url) = await _storageSvc.UploadImageAsync(bytes, fileName);
            StoragePath = sp;
            ImageUrl = url;
            _imageChanged = true;
            System.Diagnostics.Debug.WriteLine("PickImage done: " + ImageUrl);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("PickImage ERR: " + ex);
            await CommunityToolkit.Maui.Alerts.Toast.Make("Errore selezione immagine").Show();
        }
    }


    [RelayCommand]
    private async Task Save()
    {
        var tags = string.IsNullOrWhiteSpace(TagsCsv)
            ? new List<string>()
            : TagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                     .Select(t => t.Trim())
                     .Where(t => t.Length > 0)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .ToList();

        var recipe = new Recipe
        {
            Id = Id,
            Title = Title,
            Guide = Guide,
            Tips = Tips,
            PrepMinutes = PrepMinutes,
            Servings = Servings,
            ImagePath = string.IsNullOrWhiteSpace(ImageUrl) || string.IsNullOrEmpty(ImageUrl) ? null : ImageUrl,
            StoragePath = string.IsNullOrWhiteSpace(StoragePath) || string.IsNullOrEmpty(StoragePath) ? null : StoragePath,
            Tags = tags
        };

        if (string.IsNullOrEmpty(Id))
        {
            await _repo.CreateAsync(recipe);
        }
        else
        {
            await _repo.UpdateAsync(recipe);

            if (_imageChanged && !string.IsNullOrWhiteSpace(_originalStoragePath) && _originalStoragePath != StoragePath)
            {
                var store = Application.Current!.Handler!.MauiContext!.Services.GetRequiredService<IStorageService>();
                await store.DeleteAsync(_originalStoragePath!);
            }
        }

        _originalStoragePath = StoragePath;
        _imageChanged = false;

        await Shell.Current.Navigation.PopAsync();
    }

    public void Load(Recipe r)
    {
        Id = r.Id;
        Title = r.Title;
        Guide = r.Guide;
        Tips = r.Tips;
        PrepMinutes = r.PrepMinutes;
        Servings = r.Servings;
        ImageUrl = r.ImagePath ?? "";
        StoragePath = r.StoragePath ?? "";
        _originalStoragePath = r.StoragePath;
        _imageChanged = false;
        TagsCsv = r.Tags is null ? "" : string.Join(", ", r.Tags);
    }
}
