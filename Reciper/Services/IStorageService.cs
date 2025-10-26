namespace Reciper.Services;

public interface IStorageService
{
    Task<(string storagePath, string downloadUrl)> UploadImageAsync(byte[] jpegBytes, string fileName);
    Task DeleteAsync(string storagePath);
}
