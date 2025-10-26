using System.Diagnostics;
using SkiaSharp;

namespace Reciper.Services;

public class ImageService : IImageService
{
    public async Task<byte[]?> PickAndCompressJpegAsync(int maxSizePx = 1600, int quality = 80)
    {
        System.Diagnostics.Debug.WriteLine("ImageService.PickAndCompressJpegAsync");
        try
        {
            byte[]? raw = null;

            try
            {
                await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var file = await Microsoft.Maui.Media.MediaPicker.PickPhotoAsync();
                    if (file != null)
                    {
                        await using var s = await file.OpenReadAsync();
                        raw = await StreamToBytesAsync(s);
                    }
                });
            }
            catch (FeatureNotSupportedException fse)
            {
                Debug.WriteLine("MediaPicker non supportato: " + fse.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaPicker errore: " + ex.Message);
            }

            // 2) Fallback: FilePicker (funziona anche senza Photo Picker)
            if (raw is null)
            {
                var result = await Microsoft.Maui.Storage.FilePicker.PickAsync(new Microsoft.Maui.Storage.PickOptions
                {
                    PickerTitle = "Seleziona immagine",
                    FileTypes = Microsoft.Maui.Storage.FilePickerFileType.Images
                });
                if (result != null)
                {
                    await using var s = await result.OpenReadAsync();
                    raw = await StreamToBytesAsync(s);
                }
            }

            if (raw is null) return null;
            return CompressJpeg(raw, maxSizePx, quality);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("PickAndCompressJpegAsync fatal: " + ex);
            return null;
        }
    }

    static async Task<byte[]> StreamToBytesAsync(Stream s)
    {
        using var ms = new MemoryStream();
        await s.CopyToAsync(ms);
        return ms.ToArray();
    }

    static byte[]? CompressJpeg(byte[] input, int maxSizePx, int quality)
    {
        using var img = SkiaSharp.SKImage.FromEncodedData(input);
        if (img is null) return null;

        var w = img.Width; var h = img.Height;
        var scale = Math.Min(1f, (float)maxSizePx / Math.Max(w, h));
        var nw = (int)(w * scale); var nh = (int)(h * scale);

        using var surface = SkiaSharp.SKSurface.Create(new SkiaSharp.SKImageInfo(nw, nh));
        surface.Canvas.Clear();
        surface.Canvas.DrawImage(img, new SkiaSharp.SKRect(0, 0, nw, nh));
        surface.Canvas.Flush();

        using var resized = surface.Snapshot();
        using var outData = resized.Encode(SkiaSharp.SKEncodedImageFormat.Jpeg, quality);
        return outData.ToArray();
    }
}
