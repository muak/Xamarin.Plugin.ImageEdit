using System;
using System.Threading.Tasks;

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
    }
}
