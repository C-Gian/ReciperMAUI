using Reciper.Models;
using Reciper.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Reciper.Services;

public class FirestoreRecipeRepository : IRecipeRepository
{
    readonly IFirebaseAuth _auth;
    readonly HttpClient _http;
    readonly JsonSerializerOptions _jsonOpts;

    string BaseCollectionUrl =>
        $"https://firestore.googleapis.com/v1/projects/{FirebaseSecrets.ProjectId}/databases/(default)/documents/users/{_auth.UserId}/recipes";

    public FirestoreRecipeRepository(IFirebaseAuth auth)
    {
        _auth = auth;
        _http = new HttpClient();
        _jsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    void EnsureAuth()
    {
        if (string.IsNullOrEmpty(_auth.IdToken) || string.IsNullOrEmpty(_auth.UserId))
            throw new InvalidOperationException("Not authenticated.");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _auth.IdToken);
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        EnsureAuth();

        var url = $"{BaseCollectionUrl}?orderBy=updatedUtc%20desc";

        using var res = await _http.GetAsync(url);
        res.EnsureSuccessStatusCode();

        using var stream = await res.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        if (!doc.RootElement.TryGetProperty("documents", out var docs))
            return new List<Recipe>();

        var list = new List<Recipe>();
        foreach (var d in docs.EnumerateArray())
            list.Add(FirestoreMapper.FromDoc(d));

        return list;
    }

    public async Task<Recipe?> GetAsync(string id)
    {
        EnsureAuth();
        var url = $"{BaseCollectionUrl}/{id}";
        using var res = await _http.GetAsync(url);
        if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        res.EnsureSuccessStatusCode();

        using var stream = await res.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        return FirestoreMapper.FromDoc(doc.RootElement);
    }

    public async Task<string> CreateAsync(Recipe r)
    {
        EnsureAuth();
        r.CreatedUtc = DateTime.UtcNow;
        r.UpdatedUtc = r.CreatedUtc;

        var body = FirestoreMapper.ToDoc(r);
        using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");

        using var res = await _http.PostAsync(BaseCollectionUrl, content);
        res.EnsureSuccessStatusCode();

        using var stream = await res.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var created = FirestoreMapper.FromDoc(doc.RootElement);
        return created.Id!;
    }

    public async Task UpdateAsync(Recipe r)
    {
        if (string.IsNullOrWhiteSpace(r.Id))
            throw new ArgumentException("Recipe.Id required for update.");

        EnsureAuth();
        r.UpdatedUtc = DateTime.UtcNow;

        var name = $"{BaseCollectionUrl}/{r.Id}";
        var mask = "title,guide,tips,prepMinutes,servings,favorite,imagePath,ingredients,tags,updatedUtc";
        var url = $"{name}?updateMask.fieldPaths={UrlEncode(mask)}";

        var body = FirestoreMapper.ToDoc(r);
        using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");

        var req = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
        using var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string id)
    {
        EnsureAuth();
        var url = $"{BaseCollectionUrl}/{id}";
        using var res = await _http.DeleteAsync(url);
        res.EnsureSuccessStatusCode();
    }

    static string UrlEncode(string fieldList)
    {
        var fields = fieldList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0) sb.Append("&updateMask.fieldPaths=");
            sb.Append(Uri.EscapeDataString(fields[i]));
        }
        return sb.ToString();
    }
}
