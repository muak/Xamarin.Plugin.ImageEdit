using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using CoreImage;
using Foundation;
using Plugin.ImageEdit.Abstractions;
using UIKit;

namespace Plugin.ImageEdit
{
    public class EditableImage : IEditableImage
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        private UIImage _image = null;
        private bool _needToUpdateData = false;

        public EditableImage(byte[] bin)
        {
            _image = new UIImage(NSData.FromArray(bin));
            //If the image orientation is not up,reset orientation.
            if (_image.Orientation != UIImageOrientation.Up) {
                var rect = new CGRect(0, 0, _image.Size.Width, _image.Size.Height);
                UIGraphics.BeginImageContext(rect.Size);

                _image.Draw(rect);

                var newImage = UIGraphics.GetImageFromCurrentImageContext();

                UIGraphics.EndImageContext();
                _image.Dispose();
                _image = newImage;
            }
            UpdateSize();
        }

        public IEditableImage Resize(int width, int height)
        {
            int w;
            int h;
            if (width <= 0 && height <= 0) {
                return this;
            }
            else if (width <= 0) {
                h = height;
                w = (int)(height * ((double)Width / (double)Height));
            }
            else if (height <= 0) {
                w = width;
                h = (int)(width * ((double)Height / (double)Width));
            }
            else {
                w = width;
                h = height;
            }

            var rect = new CGRect(0, 0, w, h);

            UIGraphics.BeginImageContext(rect.Size);

            _image.Draw(rect);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            _image.Dispose();
            _image = newImage;
            newImage = null;

            UpdateSize();
            _needToUpdateData = true;

            return this;
        }

        public IEditableImage Resize(int maxLongSideLength)
        {
            if (Width >= Height) {
                Resize(maxLongSideLength, 0);
            }
            else {
                Resize(0, maxLongSideLength);
            }

            return this;
        }

        public IEditableImage Crop(int x, int y, int width, int height)
        {
            var rect = new CGRect(x, y, width, height);
            var crop = _image.CGImage.WithImageInRect(rect);
            _image.Dispose();
            _image = new UIImage(crop);

            UpdateSize();

            return this;
        }

        public IEditableImage Rotate(float degree)
        {
            // reference https://ruigomes.me/blog/how-to-rotate-an-uiimage-using-swift/
            //           https://oshiete.goo.ne.jp/qa/6592961.html

            var radian = -degree * Math.PI / 180f;

            //calculate canvas size
            nfloat x = Width / 2f;
            nfloat y = Height / 2f;

            var points = new List<CGPoint> { new CGPoint(x, y), new CGPoint(x, -y), new CGPoint(-x, -y), new CGPoint(-x, y) };

            for (var i = 0; i < points.Count; i++) {
                points[i] = CalculateRotatedPoint(points[i], radian);
            }

            var canvasSize = new CGSize(
                 points.Max(p => p.X) - points.Min(p => p.X),
                 points.Max(p => p.Y) - points.Min(p => p.Y)
            );


            UIGraphics.BeginImageContext(canvasSize);

            var context = UIGraphics.GetCurrentContext();
            context.TranslateCTM(canvasSize.Width / 2f, canvasSize.Height / 2f);
            context.ScaleCTM(1.0f, -1.0f);
            context.RotateCTM((nfloat)radian);
            context.DrawImage(new CGRect(-_image.Size.Width / 2f, -_image.Size.Height / 2f, _image.Size.Width, _image.Size.Height), _image.CGImage);

            var rotated = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            _image.Dispose();
            _image = rotated;

            rotated = null;
            points = null;

            _needToUpdateData = true;

            // For some reason size increase 1px when 90 degree and 270 degree 
            var absDegree = Math.Abs(degree);
            if ((Math.Abs(absDegree - 90f) <= float.Epsilon ||
                    Math.Abs(absDegree - 270f) <= float.Epsilon) &&
                (Width != (int)_image.Size.Height || Height != (int)_image.Size.Width)) {

                var fixW = Height;
                var fixH = Width;

                return Crop(0, 0, fixW, fixH);
            }

            if (Math.Abs(absDegree - 180f) <= float.Epsilon && (Width != (int)_image.Size.Width || Height != (int)_image.Size.Height)) {
                return Crop(0, 0, Width, Height);
            }


            UpdateSize();
            return this;
        }

        public IEditableImage ToMonochrome()
        {
            var mono = new CIColorMonochrome {
                Color = CIColor.FromRgb(1, 1, 1),
                Intensity = 1.0f,
                Image = CIImage.FromCGImage(_image.CGImage)
            };
            CIImage output = mono.OutputImage;
            var context = CIContext.FromOptions(null);
            var renderedImage = context.CreateCGImage(output, output.Extent);
            var monoImage = UIImage.FromImage(renderedImage, _image.CurrentScale, _image.Orientation);

            _image.Dispose();
            _image = monoImage;

            monoImage = null;
            renderedImage.Dispose();
            renderedImage = null;

            return this;
        }

        public byte[] ToJpeg(float quality = 80)
        {
            using (var data = _image.AsJPEG(quality)) {
                return data.ToArray();
            }
        }

        public byte[] ToPng()
        {
            using (var data = _image.AsPNG()) {
                return data.ToArray();
            }
        }

        public int[] ToArgbPixels()
        {
            //DataProvider.CopyData don't update when image rotated and resized.
            if (_needToUpdateData) {
                using (var wkimage = new UIImage(NSData.FromArray(_image.AsPNG().ToArray())))
                using (var data = wkimage.CGImage.DataProvider.CopyData()) {
                    _needToUpdateData = false;
                    return GetBitmapPixels(data, wkimage.CGImage.AlphaInfo);
                }
            }

            using (var data = _image.CGImage.DataProvider.CopyData()) {
                return GetBitmapPixels(data, _image.CGImage.AlphaInfo);
            }
        }

        int[] GetBitmapPixels(NSData data, CGImageAlphaInfo alphaInfo)
        {
            var adjustOrder = 2;
            if (alphaInfo == CGImageAlphaInfo.First ||
                alphaInfo == CGImageAlphaInfo.PremultipliedFirst) {
                adjustOrder = 0;
            }

            var bytesPerPixel = _image.CGImage.BitsPerPixel / 8;

            // Order by ARGB
            var pixels = new int[Width * Height];
            var idx = 0;
            for (var i = 0; i < Height; i++) {
                for (var j = 0; j < Width; j++) {
                    var addr = (i * Width + j) * bytesPerPixel;
                    pixels[idx++] =
                        (data[addr + 3] << 24) |
                        (data[addr + 2 - adjustOrder] << 16) |
                        (data[addr + 1] << 8) |
                        (data[addr + adjustOrder]);
                }
            }


            return pixels;
        }

        public void Dispose()
        {
            _image.Dispose();
            _image = null;
        }

        CGPoint CalculateRotatedPoint(CGPoint p, double radian)
        {
            return new CGPoint(
                p.X * Math.Cos(radian) - p.Y * Math.Sin(radian),
                p.X * Math.Sin(radian) + p.Y * Math.Cos(radian)
            );
        }

        void UpdateSize()
        {
            Width = (int)_image.CGImage.Width;
            Height = (int)_image.CGImage.Height;
        }
    }
}
