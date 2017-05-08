using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.IO;
using Xamarin.Forms;

namespace Sample.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
		private ImageSource _Image;
		public ImageSource Image {
			get { return _Image; }
			set { SetProperty(ref _Image, value); }
		}

		private byte[] _data;

		private DelegateCommand _PickCommand;
		public DelegateCommand PickCommand { 
		    get { return _PickCommand = _PickCommand ?? new DelegateCommand(async()=>{
				var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync();
				_data = await MediaFileToBytes(file);
				Image = ImageSource.FromStream(()=>file.GetStream());
			}); } 
		}

		private DelegateCommand _ToMonochromeCommand;
		public DelegateCommand ToMonochromeCommand {
			get { return _ToMonochromeCommand = _ToMonochromeCommand ?? new DelegateCommand(async () => {
				using(var image = await Plugin.ImageEdit.CrossImageEdit.Current.CreateImageAsync(_data)){
					Image = ImageSource.FromStream(()=>new MemoryStream(image.ToMonochrome().ToJpeg(100)));
				}
			}); }
		}

		public MainPageViewModel()
		{
		}

		async Task<byte[]> MediaFileToBytes(MediaFile file)
		{
			using (var ms = new MemoryStream()) {
				var buff = new byte[16 * 1024];
				int read;
				var stream = file.GetStream();
				while ((read = await stream.ReadAsync(buff, 0, buff.Length)) > 0) {
					await ms.WriteAsync(buff, 0, read);
				}

				return ms.ToArray();
			}
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
			
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
		}
	}
}

