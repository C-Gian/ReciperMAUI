using System.Net.Http.Json;
using Reciper.Utils;

namespace Reciper.Services;

public class FirebaseAuth : IFirebaseAuth
{
    readonly HttpClient _http = new();
    public string? IdToken { get; private set; }
    public string? UserId { get; private set; }
    DateTime _expiresAtUtc;

    record SignInReq(string email, string password, bool returnSecureToken = true);
    record SignInRes(string idToken, string localId, string refreshToken, string expiresIn);

    public async Task<bool> SignInAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseSecrets.ApiKey}";
        var res = await _http.PostAsJsonAsync(url, new SignInReq(email, password));
        if (!res.IsSuccessStatusCode) return false;

        var data = await res.Content.ReadFromJsonAsync<SignInRes>();
        if (data is null) return false;

        IdToken = data.idToken;
        UserId = data.localId;
        _expiresAtUtc = DateTime.UtcNow.AddSeconds(int.Parse(data.expiresIn));
        return true;
    }
}
