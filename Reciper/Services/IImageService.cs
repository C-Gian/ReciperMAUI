namespace Reciper.Services;

public interface IImageService
{
    Task<byte[]?> PickAndCompressJpegAsync(int maxSizePx = 1600, int quality = 80);
}
