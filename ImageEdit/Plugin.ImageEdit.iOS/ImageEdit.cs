using System.Threading.Tasks;
using Plugin.ImageEdit.Abstractions;

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
    }
}