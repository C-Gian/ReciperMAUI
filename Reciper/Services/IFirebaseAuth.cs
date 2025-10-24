namespace Reciper.Services;
public interface IFirebaseAuth
{
    string? IdToken { get; }
    string? UserId { get; }
    Task<bool> SignInAsync(string email, string password);
}