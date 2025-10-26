using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Reciper.Models;
using System.Collections.ObjectModel;

namespace Reciper.ViewModels;

public partial class RecipesViewModel : ObservableObject
{
    #region Property

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string query = string.Empty;

    [ObservableProperty]
    private string? selectedTag;

    private readonly IRecipeRepository _repo;
    public ObservableCollection<TagFilterItem> Tags { get; } = new();
    public ObservableCollection<Recipe> Items { get; } = new();

    private List<Recipe> _all = new();

    #endregion


    public RecipesViewModel(IRecipeRepository repo)
    {
        _repo = repo;
    }

    [RelayCommand]
    private void ToggleTag(string name)
    {
        var all = Tags.First(x => x.Name == "Tutti");
        if (name == "Tutti")
        {
            // seleziona solo "Tutti"
            foreach (var t in Tags) t.IsSelected = (t == all);
        }
        else
        {
            var tag = Tags.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            tag.IsSelected = !tag.IsSelected;
            all.IsSelected = false;
            if (!Tags.Any(t => t.Name != "Tutti" && t.IsSelected))
                all.IsSelected = true;
        }
        ApplyFilter();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            _all = await _repo.GetAllAsync();

            Tags.Clear();
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in _all)
                if (r.Tags != null)
                    foreach (var t in r.Tags)
                        if (!string.IsNullOrWhiteSpace(t)) set.Add(t.Trim());

            Tags.Clear();
            Tags.Add(new TagFilterItem("Tutti", selected: true));
            foreach (var t in set.OrderBy(s => s)) Tags.Add(new TagFilterItem(t));

            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    partial void OnQueryChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var q = (Query ?? string.Empty).Trim().ToLowerInvariant();
        var active = Tags.Where(t => t.Name != "Tutti" && t.IsSelected)
                     .Select(t => t.Name)
                     .ToHashSet(StringComparer.OrdinalIgnoreCase);

        IEnumerable<Recipe> src = _all;

        if (active.Count > 0)
        {
            src = src.Where(r => (r.Tags ?? new()).Any(t => active.Contains(t)));
        }

        if (q.Length > 0)
        {
            src = _all.Where(r =>
                (r.Title ?? "").ToLowerInvariant().Contains(q) ||
                (r.Guide ?? "").ToLowerInvariant().Contains(q) ||
                (r.Tips ?? "").ToLowerInvariant().Contains(q) ||
                (r.Tags ?? new()).Any(t => t.ToLowerInvariant().Contains(q)) ||
                (r.Ingredients ?? new()).Any(i => i.ToLowerInvariant().Contains(q))
            );
        }

        Items.Clear();
        foreach (var r in src) Items.Add(r);
    }

    partial void OnSelectedTagChanged(string? value) => ApplyFilter();

    [RelayCommand]
    public async Task RefreshAsync() => await LoadAsync();
}

public partial class TagFilterItem : ObservableObject
{
    public string Name { get; }

    [ObservableProperty] 
    private bool isSelected;

    public TagFilterItem(string name, bool selected = false)
    {
        Name = name;
        IsSelected = selected;
    }
}
