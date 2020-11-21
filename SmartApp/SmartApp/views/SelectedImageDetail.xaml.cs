using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectedImageDetail : ContentPage
	{
        System.IO.Stream seletedimg = null;
        public SelectedImageDetail ()
		{
			InitializeComponent ();
		}

        public SelectedImageDetail(System.IO.Stream file) : this()
        {
            seletedimg = file;
            image.Source = ImageSource.FromStream(() =>
            {
                var stream = file;
                //file.Dispose();
                return stream;
            });
        }

        private void btn_Return(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ImageDetail());            
        }

        private void btn_SaveImageInfo(object sender, EventArgs e)
        {
            
        }
    }
}