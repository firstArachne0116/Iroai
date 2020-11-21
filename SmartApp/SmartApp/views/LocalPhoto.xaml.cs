using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LocalPhoto : ContentPage
	{
		public LocalPhoto ()
		{
			InitializeComponent ();
		}

        private void btn_Return(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new MainPage());
        }

        private void btn_SaveImageInfo(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SelectedImageDetail());
        }
    }
}