using System.Threading.Tasks;
using Plugin.ImageEdit.Abstractions;
using System.IO;
using ImageIO;
using Foundation;
using System;
using UIKit;

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

        public async Task<IEditableImage> CreateImageAsync(byte[] imageArray,int downWidth,int downHeight)
        {

            return await Task.Run(() => {

                var imageSource = CGImageSource.FromData(NSData.FromArray(imageArray), new CGImageOptions { ShouldCache = false });

                var downsampledImage = imageSource.CreateThumbnail(0, new CGImageThumbnailOptions {
                    ShouldCache = true,
                    ShouldCacheImmediately = true,
                    CreateThumbnailWithTransform = true,
                    MaxPixelSize = Math.Max(downWidth, downHeight),
                });

                return new EditableImage(new UIImage(downsampledImage));
            });
        }
    }
}