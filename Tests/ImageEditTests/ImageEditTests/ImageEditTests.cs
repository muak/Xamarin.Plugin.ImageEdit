using System;
using Xunit;
using System.Threading.Tasks;
using Plugin.ImageEdit.Abstractions;
using Plugin.ImageEdit;
#if __ANDROID__
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.Security.AccessControl;
using Java.IO;
using System.IO;
#endif
#if __IOS__
using UIKit;
using Foundation;
using System.IO;
#endif

namespace Tests
{
    public class MainFixture : IDisposable
    {
        public byte[] PngData { get; set; }
        public byte[] JpegData { get; set; }
        public byte[] ShrinkingData { get; set; }
        public byte[] ExpansionData { get; set; }
        public byte[] ColorsData { get; set; }
        public byte[] RectH { get; set; }
        public byte[] RectV { get; set; }

        public MainFixture()
        {
#if __IOS__
            PngData = UIImage.FromBundle("pattern1.png").AsPNG().ToArray();
            JpegData = UIImage.FromBundle("pattern1.jpg").AsJPEG().ToArray();
            ShrinkingData = UIImage.FromBundle("shrinking.png").AsPNG().ToArray();
            ExpansionData = UIImage.FromBundle("expansion.png").AsPNG().ToArray();
            ColorsData = UIImage.FromBundle("colors.png").AsPNG().ToArray();
            RectH = UIImage.FromBundle("recth.png").AsPNG().ToArray();
            RectV = UIImage.FromBundle("rectv.png").AsPNG().ToArray();
#endif
#if __ANDROID__
            PngData = Getbytes("pattern1");
            JpegData = Getbytes("patternjpg");
            ShrinkingData = Getbytes("shrinking");
            ExpansionData = Getbytes("expansion");
            ColorsData = Getbytes("colors");
            RectH = Getbytes("recth");
            RectV = Getbytes("rectv");
#endif

        }

#if __ANDROID__
        byte[] Getbytes(string name)
        {
            var con = Android.App.Application.Context;
            var res = con.Resources.GetIdentifier(name, "drawable", con.PackageName);

            using (var raw = con.Resources.OpenRawResource(res))
            using (var stream = new ByteArrayOutputStream()) {
                var buff = new byte[10240];
                var i = int.MaxValue;
                while ((i = raw.Read(buff, 0, buff.Length)) > 0) {
                    stream.Write(buff, 0, i);
                }

                return stream.ToByteArray();
            }
        }
#endif

        public void Dispose()
        {

        }
    }

    public class Main : IClassFixture<MainFixture>
    {
        MainFixture _main;
        IImageEdit _editor;
        public Main(MainFixture main)
        {
            _main = main;
            _editor = new ImageEdit();
        }

        [Fact]
        public void CreateImageTest()
        {
            using (var image = _editor.CreateImage(_main.PngData)) {
                CreateCompare(image);
            }
            using (var image = _editor.CreateImage(_main.JpegData)) {
                CreateCompare(image, false);
            }
        }

        [Fact]
        public async Task CreateImageAsyncTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {
                CreateCompare(image);
            }
            using (var image = await _editor.CreateImageAsync(_main.JpegData)) {
                CreateCompare(image, false);
            }

            var stream = new MemoryStream(_main.PngData);
            using (var image = await _editor.CreateImageAsync(stream)) {
                CreateCompare(image);
                Assert.Throws<ObjectDisposedException>(() => {
                    var data = stream.Length;
                });
            }
        }

        void CreateCompare(IEditableImage image, bool isPng = true)
        {
            var array = image.ToArgbPixels();

            Assert.Equal(3, image.Width);
            Assert.Equal(3, image.Height);

            if (isPng) {
                for (var i = 0; i < 3; i++) {
                    Assert.Equal(0xFF000000, (uint)array[i]);
                }
                for (var i = 3; i < array.Length; i++) {
                    Assert.NotEqual(0xFF000000, (uint)array[i]);
                }
            }
        }

        [Fact]
        public async Task Rotate90Test()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(90).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if ((i + 1) % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }


        [Fact]
        public void Rotate180Test()
        {
            using (var image = _editor.CreateImage(_main.PngData)) {

                var pixels = image.Rotate(180).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i >= 6) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task Rotate270Test()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(270).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task Rotate90negativeTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(-90).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task CropTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Crop(1, 1, 1, 1).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 1);
                Assert.Equal(image.Height, 1);

                Assert.Equal(0xFFFF0000, (uint)pixels[0]);
            }
        }

        [Fact]
        public async Task ResizeShrinkingTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.ShrinkingData)) {

                var pixels = image.Resize(2, 2).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 2);
                Assert.Equal(image.Height, 2);

                // approximate white
                Assert.InRange((uint)pixels[0], 0xFFE0E0E0, 0xFFFFFFFF);
                Assert.InRange((uint)pixels[1], 0xFFE0E0E0, 0xFFFFFFFF);
                Assert.InRange((uint)pixels[2], 0xFFE0E0E0, 0xFFFFFFFF);
                // approximate black
                Assert.InRange((uint)pixels[3], 0xFF000000, 0xFF3F3F3F);
            }
        }

        [Fact]
        public async Task ResizeExpansionTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.ExpansionData)) {

                var pixels = image.Resize(4, 4).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 4);
                Assert.Equal(image.Height, 4);

                for (var i = 0; i < image.Height; i++) {
                    for (var j = 0; j < image.Width; j++) {
                        if (i >= 2 && j >= 2) {
                            // approximate black  (very loose for android)
                            Assert.InRange((uint)pixels[i * image.Width + j], 0xFF000000, 0xFF6F6F6F);
                        }
                        else {
                            // approximate white (very loose for android)
                            Assert.InRange((uint)pixels[i * image.Width + j], 0xFFBFBFBF, 0xFFFFFFFF);
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task ResizeSpecifyLongSideTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.RectH)) {
                image.Resize(5);

                Assert.Equal(5, image.Width);
                Assert.Equal(3, image.Height);
            }

            using (var image = await _editor.CreateImageAsync(_main.RectV)) {
                image.Resize(5);

                Assert.Equal(3, image.Width);
                Assert.Equal(5, image.Height);
            }
        }

        [Fact]
        public async Task ToMonochromeTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.ColorsData)) {
                var pixels = image.ToMonochrome().ToArgbPixels();

                SavePhoto(image);

                foreach (var pixel in pixels) {
                    var r = pixel & 0x00FF0000 >> 16;
                    var g = pixel & 0x0000FF00 >> 8;
                    var b = pixel & 0x000000FF;

                    Assert.InRange(r - g, -10, 10);
                    Assert.InRange(r - b, -10, 10);
                }
            }
        }

        [Fact]
        public async Task GetNativeObjectTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {
                var obj = image.GetNativeImage();
                var pArray = image.ToPng();
#if __IOS__
                var nimage = obj as UIImage;
                var nArray = nimage.AsPNG().ToArray();

                Assert.Equal(nArray,pArray);
#elif __ANDROID__
                var nimage = obj as Bitmap;
                using (var ms = new MemoryStream()) {
                    nimage.Compress(Bitmap.CompressFormat.Png, 100, ms);
                    var nArray = ms.ToArray();
                    Assert.Equal(nArray, pArray);
                }
#endif
            }
        }

        private void SavePhoto(IEditableImage image)
        {
#if __IOS__
            var tmp = new UIImage(NSData.FromArray(image.ToPng()));
            tmp.BeginInvokeOnMainThread(() => {
                tmp.SaveToPhotosAlbum(new UIImage.SaveStatus((UIImage affs, NSError error) => {
                    ;
                }));
            });
#endif
#if __ANDROID__

#endif
        }
    }
}
