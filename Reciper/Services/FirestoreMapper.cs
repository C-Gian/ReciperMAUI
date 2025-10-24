using Reciper.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Reciper.Services;

static class FirestoreMapper
{
    // Recipe -> Firestore document
    public static JsonObject ToDoc(Recipe r)
    {

        JsonArray Arr(IEnumerable<string> items)
        {
            var nodes = new List<JsonNode?>();
            foreach (var s in items)
            {
                if (string.IsNullOrWhiteSpace(s)) continue;
                nodes.Add(new JsonObject { ["stringValue"] = s });
            }
            return new JsonArray(nodes.ToArray());
        }


        var f = new JsonObject
        {
            ["title"] = S(r.Title),
            ["guide"] = S(r.Guide),
            ["tips"] = S(r.Tips),
            ["prepMinutes"] = I(r.PrepMinutes),
            ["servings"] = I(r.Servings),
            ["favorite"] = B(r.Favorite),
            ["imagePath"] = r.ImagePath is null ? Null() : S(r.ImagePath),
            ["ingredients"] = new JsonObject { ["arrayValue"] = new JsonObject { ["values"] = Arr(r.Ingredients) } },
            ["tags"] = new JsonObject { ["arrayValue"] = new JsonObject { ["values"] = Arr(r.Tags) } },
            ["createdUtc"] = T(r.CreatedUtc),
            ["updatedUtc"] = T(r.UpdatedUtc)
        };

        return new JsonObject { ["fields"] = f };

        static JsonObject S(string v) => new() { ["stringValue"] = v };
        static JsonObject I(int v) => new() { ["integerValue"] = v.ToString() };
        static JsonObject B(bool v) => new() { ["booleanValue"] = v };
        static JsonObject T(DateTime utc) => new() { ["timestampValue"] = utc.ToUniversalTime().ToString("O") };
        static JsonObject Null() => new() { ["nullValue"] = JsonValue.Create((string?)null)! };
    }

    // Firestore document -> Recipe
    public static Recipe FromDoc(JsonElement doc)
    {
        string? NameToId(string name)
            => string.IsNullOrEmpty(name) ? null : name.Split('/').Last();

        var fields = doc.GetProperty("fields");

        return new Recipe
        {
            Id = NameToId(doc.GetProperty("name").GetString()!),
            Title = fields.GetProperty("title").GetProperty("stringValue").GetString() ?? "",
            Guide = fields.GetProperty("guide").GetProperty("stringValue").GetString() ?? "",
            Tips = fields.GetProperty("tips").GetProperty("stringValue").GetString() ?? "",
            PrepMinutes = int.Parse(fields.GetProperty("prepMinutes").GetProperty("integerValue").GetString() ?? "0"),
            Servings = int.Parse(fields.GetProperty("servings").GetProperty("integerValue").GetString() ?? "1"),
            Favorite = fields.GetProperty("favorite").GetProperty("booleanValue").GetBoolean(),
            ImagePath = fields.TryGetProperty("imagePath", out var ip)
                            && ip.TryGetProperty("stringValue", out var sv)
                            ? sv.GetString()
                            : null,
            Ingredients = ReadArray(fields, "ingredients"),
            Tags = ReadArray(fields, "tags"),
            CreatedUtc = DateTime.Parse(fields.GetProperty("createdUtc").GetProperty("timestampValue").GetString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal),
            UpdatedUtc = DateTime.Parse(fields.GetProperty("updatedUtc").GetProperty("timestampValue").GetString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal),
        };

        static List<string> ReadArray(JsonElement f, string key)
        {
            if (!f.TryGetProperty(key, out var el) || !el.TryGetProperty("arrayValue", out var av))
                return new();
            if (!av.TryGetProperty("values", out var vals))
                return new();
            return vals.EnumerateArray()
                       .Select(v => v.GetProperty("stringValue").GetString() ?? "")
                       .Where(s => s.Length > 0)
                       .ToList();
        }
    }
}
