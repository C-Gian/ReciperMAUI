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
}