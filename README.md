# Image Edit Plugin for Xamarin

This plugin will enable you to manipulate(resize,crop,rotate) a image(png,jpg).

### Setup

* Available on NuGet: https://www.nuget.org/packages/Xamarin.Plugin.ImageEdit/
* Install into your PCL project and Client projects.

```bash
Install-Package Xamarin.Plugin.ImageEdit -Pre
```

**Platform Support**

|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 9+|
|Xamarin.Android|Yes|API 22+|
|Windows 10 UWP|No||
|Xamarin.Mac|No||

## Usage example

Image crop and rotate and resize and get png data.

### Using as plugin

```cs
using (var image = await CrossImageEdit.Current.CreateImageAsync(imageByteArray)) {
	var croped = await Task.Run(() =>
			image.Crop(10, 20, 250, 100)
				 .Rotate(180)
				 .Resize(100, 0)
				 .ToPng()
	);
}
```

### Using as IPlatformInitializer(prism.unity.forms)

```cs
//View model constructor
public ViewModel(IImageEdit imageEdit){

	using (var image = await imageEdit.CreateImageAsync(imageByteArray)) {
		var croped = await Task.Run(() =>
				image.Crop(10, 20, 250, 100)
					 .Rotate(180)
					 .Resize(100, 0)
					 .ToPng()
		);
	}
}


//on platform
public class iOSInitializer : IPlatformInitializer
{
	public void RegisterTypes(IUnityContainer container)
	{
		container.RegisterType<IImageEdit,ImageEdit>();
	}
}
```

### Sample project

https://github.com/muak/PanPinchSample

**movie**
https://twitter.com/muak_x/status/837266085405573120

## API Usage

### Get EditableImage

```cs
var image = await CrossImageEdit.Current.CreateImageAsync(imageByteArray);
```
It is able to manipulate a image using this object.

### Resize

```cs
var width = 200;
var height = 200;
image.Resize(width, height);
image.Resize(width, 0); //auto height
image.Resize(0, height); //auto width
```

### Crop

```cs
var x = 10;
var y = 10;
var width = 50;
var height = 50;
image.Crop(10, 10, 50, 50);
```

### Rotate

```cs
var degree = 90; // 0-360;
image.Rotate(degree);
```

### ToPng

```cs
var pngBytes = image.ToPng();
```

### ToJpeg

```cs
var jpgBytes = image.ToJpeg(90); // quality(0-100)
```

### ToArgbPixels

Get image ARGB infomation.

for example when 0xFF00F090

|A|R|G|B|
| :--- | :--- | :--- | :--- |
|FF|00|F0|90|


```cs
var pixels = image.ToArgbPixels();
```

## License

MIT Licensed.
