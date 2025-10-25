namespace Reciper.Models;
public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync();
    Task<Recipe?> GetAsync(string id);
    Task<string> CreateAsync(Recipe recipe);  
    Task UpdateAsync(Recipe recipe);        
    Task DeleteAsync(string id);
}