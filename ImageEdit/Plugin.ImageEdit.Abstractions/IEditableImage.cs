using System;
namespace Plugin.ImageEdit.Abstractions
{
    public interface IEditableImage:IDisposable
    {
        int Width { get; }
        int Height { get; }
        IEditableImage Resize(int width, int height);
        IEditableImage Crop(int x, int y, int width, int height);
        IEditableImage Rotate(float degree);
        byte[] ToJpeg(float quality = 80);
        byte[] ToPng();
        int[] ToArgbPixels();
    }
}
