using System;
namespace Plugin.ImageEdit.Abstractions
{
    /// <summary>
    /// Interface for EditableImage
    /// </summary>
    public interface IEditableImage:IDisposable
    {
        /// <summary>
        /// Image Width
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Image Height
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Resize 
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="width">resize width. 0 is adjust to aspect ratio.</param>
        /// <param name="height">resize height. 0 is adjust to aspect ratio.</param>
        IEditableImage Resize(int width, int height);

        /// <summary>
        /// Crop
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="x">start x</param>
        /// <param name="y">start y</param>
        /// <param name="width">crop width</param>
        /// <param name="height">crop height</param>
        IEditableImage Crop(int x, int y, int width, int height);

        /// <summary>
        /// Rotate
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="degree">degree(0-360)</param>
        IEditableImage Rotate(float degree);

        /// <summary>
        /// To Jpeg byte array
        /// </summary>
        /// <returns>byte[]</returns>
        /// <param name="quality">quality(1-100)</param>
        byte[] ToJpeg(float quality = 80);

        /// <summary>
        /// To PNG byte array
        /// </summary>
        /// <returns>byte[]</returns>
        byte[] ToPng();

        /// <summary>
        /// image pixels array. order by ARGB. 0xFF(A)FF(R)FF(G)FF(B)
        /// </summary>
        /// <returns>The ARGB pixels.</returns>
        int[] ToArgbPixels();
    }
}
