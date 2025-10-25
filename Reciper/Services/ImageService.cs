using SkiaSharp;

namespace Reciper.Services;

public class ImageService : IImageService
{
    public async Task<byte[]?> PickAndCompressJpegAsync(int maxSizePx = 1600, int quality = 80)
    {
        var file = await MediaPicker.PickPhotoAsync();
        if (file is null) return null;

        await using var s = await file.OpenReadAsync();
        using var data = SKData.Create(s);
        using var img = SKImage.FromEncodedData(data);
        if (img is null) return null;

        var w = img.Width; var h = img.Height;
        var scale = Math.Min(1f, (float)maxSizePx / Math.Max(w, h));
        var nw = (int)(w * scale); var nh = (int)(h * scale);

        using var surface = SKSurface.Create(new SKImageInfo(nw, nh));
        surface.Canvas.Clear();
        surface.Canvas.DrawImage(img, new SKRect(0, 0, nw, nh));
        surface.Canvas.Flush();

        using var resized = surface.Snapshot();
        using var outData = resized.Encode(SKEncodedImageFormat.Jpeg, quality);
        return outData.ToArray();
    }
}