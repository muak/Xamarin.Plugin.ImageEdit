using System;
using System.Threading.Tasks;
using System.IO;

namespace Plugin.ImageEdit.Abstractions
{
    /// <summary>
    /// Interface for ImageEdit
    /// </summary>
    public interface IImageEdit
    {
        /// <summary>
        /// Create editable image
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="imageArray">image byte array</param>
        IEditableImage CreateImage(byte[] imageArray);

        /// <summary>
        /// Create editable image (async)
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="imageArray">image byte array</param>
        Task<IEditableImage> CreateImageAsync(byte[] imageArray);

        /// <summary>
        /// Creates the image async.
        /// </summary>
        /// <returns>IEditableImage</returns>
        /// <param name="stream">image stream. the stream will be disposed by this method.</param>
        Task<IEditableImage> CreateImageAsync(Stream stream);
    }
}
