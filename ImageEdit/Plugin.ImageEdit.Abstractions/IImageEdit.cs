using System;
using System.Threading.Tasks;

namespace Plugin.ImageEdit.Abstractions
{
    /// <summary>
    /// Interface for ImageEdit
    /// </summary>
    public interface IImageEdit
    {
        IEditableImage CreateImage(byte[] imageArray);
        Task<IEditableImage> CreateImageAsync(byte[] imageArray);
    }
}
