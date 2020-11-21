using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPage : ContentPage
	{
		public CameraPage ()
		{
			InitializeComponent ();
		}

        public CameraPage(MediaFile file) : this()
        {
            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });
        }
    }
}