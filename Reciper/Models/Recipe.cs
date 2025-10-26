namespace Reciper.Models;
public class Recipe
{
    public string? Id { get; set; }            
    public string? ImagePath { get; set; }   
    public string Title { get; set; } = "";
    public string Guide { get; set; } = "";
    public string Tips { get; set; } = "";
    public int PrepMinutes { get; set; }
    public int Servings { get; set; } = 1;
    public List<string> Ingredients { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public bool Favorite { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    public string? StoragePath { get; set; } 
}