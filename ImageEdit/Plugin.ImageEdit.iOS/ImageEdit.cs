using System.Threading.Tasks;
using Plugin.ImageEdit.Abstractions;
using System.IO;

namespace Plugin.ImageEdit
{
    /// <summary>
    /// Implementation for ImageEdit
    /// </summary>
    public class ImageEdit : IImageEdit
    {
        public IEditableImage CreateImage(byte[] imageArray)
        {
            return new EditableImage(imageArray);
        }

        public async Task<IEditableImage> CreateImageAsync(byte[] imageArray)
        {
            return await Task.Run(() => CreateImage(imageArray));
        }

        public async Task<IEditableImage> CreateImageAsync(Stream stream)
        {
            return await CreateImageAsync(await Utilities.StreamToBytes(stream));
        }
    }
}