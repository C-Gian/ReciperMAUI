using System.Net.Http.Headers;
using System.Text.Json;
using Reciper.Utils;

namespace Reciper.Services;

public class FirebaseStorageService : IStorageService
{
    readonly IFirebaseAuth _auth;
    readonly HttpClient _http = new();

    public FirebaseStorageService(IFirebaseAuth auth) { _auth = auth; }

    public async Task<(string storagePath, string downloadUrl)> UploadImageAsync(byte[] jpegBytes, string fileName)
    {
        if (string.IsNullOrEmpty(_auth.IdToken) || string.IsNullOrEmpty(_auth.UserId))
            throw new InvalidOperationException("Not authenticated.");

        // path nel bucket
        var path = $"users/{_auth.UserId}/images/{fileName}";
        var url = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseSecrets.StorageBucket}/o?name={Uri.EscapeDataString(path)}";

        using var content = new ByteArrayContent(jpegBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.TryAddWithoutValidation("Authorization", $"Firebase {_auth.IdToken}");
        req.Content = content;

        using var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var token = doc.RootElement.GetProperty("downloadTokens").GetString();
        var dl = $"https://firebasestorage.googleapis.com/v0/b/{FirebaseSecrets.StorageBucket}/o/{Uri.EscapeDataString(path)}?alt=media&token={token}";
        return (path, dl);
    }
}
