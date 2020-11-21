using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartApp.views
{
    public class ExtendedImage : Image
    {
        public ExtendedImage()
        {
        }

        public static BindableProperty ImageTappedCommandProperty =
            BindableProperty.Create<ExtendedImage, ICommand>(p => p.ImageTappedCommand, default(ICommand));

        public static BindableProperty ImageTappedParameterProperty =
            BindableProperty.Create<ExtendedImage, object>(p => p.ImageTappedParameter, default(object));



        public static readonly BindableProperty XEventProperty =
            BindableProperty.Create<ExtendedImage, float>(p => p.XEvent, 0);
        public static readonly BindableProperty YEventProperty =
            BindableProperty.Create<ExtendedImage, float>(p => p.YEvent, 0);

        public float XEvent { get { return (float)base.GetValue(XEventProperty); } set { base.SetValue(XEventProperty, value); } }
        public float YEvent { get { return (float)base.GetValue(YEventProperty); } set { base.SetValue(YEventProperty, value); } }

        public object ImageTappedParameter { get { return (object)GetValue(ImageTappedParameterProperty); } set { SetValue(ImageTappedParameterProperty, value); } }

        public ICommand ImageTappedCommand
        {
            get { return (ICommand)GetValue(ImageTappedCommandProperty); }
            set { SetValue(ImageTappedCommandProperty, value); }
        }


        public void ImageTap(ImageCoordsEvent e)
        {
            XEvent = e.x;
            YEvent = e.y;
            if (ImageTappedCommand != null)
            {
                if (ImageTappedParameter != null)
                    ImageTappedCommand.Execute(ImageTappedParameter);
                else
                    ImageTappedCommand.Execute(e);
            }
        }
    }


    public class ImageCoordsEvent : EventArgs
    {
        public float x { get; set; }

        public float y { get; set; }
    }
}
