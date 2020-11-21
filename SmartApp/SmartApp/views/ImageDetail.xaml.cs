using Acr.UserDialogs;
using AdMob.DependencyServices;
using Android.App;
using Microsoft.AppCenter.Crashes;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SmartApp.ColorPick;
using SmartApp.Controls;
using SmartApp.Interface;
using SmartApp.Model;
using SmartApp.TouchTracking;
using SmartApp.Util;
using SmartApp.views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageDetail : ContentPage
    {
        ArrayList arrayList = new ArrayList();
        // 設定ファイル
        Dictionary<string, string> dicConfig = new Dictionary<string, string>();
        // カラー配列
        List<ImageColorInfo> _listinfo = new List<ImageColorInfo>();
        // 選択カラー
        System.IO.Stream seletedimg = null;
        // 画像
        SKBitmap bitmap = null;

        private int _imageID = -1;
        private string TitleName = "";

        private bool _onCapture = false;
        private bool _colorChanged;

        //private string _ImagePath;
        private long touchId;
        private Xamarin.Forms.Point pressPoint1;
        private Xamarin.Forms.Point pressPoint2;
        DateTime pressTime;
        long iTime = 0;
        // 移動後X座標
        private double lastXMove = 0;
        // 移動後Y座標
        private double lastYMove = 0;
        // 前回移動後X座標
        private double lastXMoveBak = 0;
        // 前回移動後Y座標
        private double lastYMoveBak = 0;
        // 2指の間隔
        private double lastFingerDis;
        // 拡大・縮小の倍率
        private double scaledRatio = 1;
        // 画像とレンズのデフォルト倍率
        private double scaledRatioDef = 1;
        private int defX = 0;
        private int defY = 0;

        Xamarin.Forms.Point _point = new Xamarin.Forms.Point(0.0, 0.0);
        List<ImageColorInfo> qList = new List<ImageColorInfo>();

        int inputtype = 1;

        MediaFile currentFile;
        byte[] CurrentbyteArray;


        string _fullFilePath = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public ImageDetail()
        {
            InitializeComponent();
            Config();
            if (gridCanvas.HeightRequest * 2 > App.ScreenHeight)
            {
                gridCanvas.HeightRequest = App.ScreenHeight / 2;
            }
            App.srcPage = this;
        }

        /// <summary>
        /// Congigファイル読み込み
        /// </summary>
        void Config()
        {
            try
            {
                string text = DependencyService.Get<IData>().GetData("config.txt");
                string[] line = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length > 0)
                {
                    foreach (string str in line)
                    {
                        string[] part = str.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length > 0)
                        {
                            dicConfig.Add(part[0], part[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected override async void OnAppearing()
        {

            base.OnAppearing();

            if (TitleName != "")
            {
                List<CataLogInfo> catologInfoList = await SqliteUtil.Current.QueryCataLogsByTitle(TitleName);

                if (catologInfoList == null || catologInfoList.Count == 0)
                {
                    await DisplayAlert("", "データがありません。", "OK");
                    App.Current.MainPage = App.mainPage;
                }
            }

            // カンバス更新
            reload();
        }

        public void reflash()
        {

            qList.Clear();

            //pickColorList.Children.Clear();

            // カンバス更新
            reload();
        }

        public void reload()
        {
            ColorCode colorcd = new ColorCode();

            if (qList.Count > 0)
            {
                string hex = qList[qList.Count - 1].HEXValue;
                btnColor.BackgroundColor = Xamarin.Forms.Color.FromHex(hex);
                lblSelected.Text = hex;
                lblHex.Text = hex;

                Rgb rgb = colorcd.getRgb(hex);
                lblRgb.Text = colorcd.getRgbString(rgb);
                lblHsl.Text = colorcd.getHslString(rgb);
                lblHsv.Text = colorcd.getHsvString(rgb);
                lblCmyk.Text = colorcd.getCmykString(rgb);
                lblLab.Text = colorcd.getLabString(rgb);
                lblMunsell.Text = colorcd.getMunsellString(rgb);
                lblPccs.Text = colorcd.getPCCSTone(rgb);
                lblJis.Text = JisUtil.getJisByMunsell(lblMunsell.Text);


            }
            else
            {
                btnColor.BackgroundColor = Xamarin.Forms.Color.White;
                lblSelected.Text = string.Empty;
                lblHex.Text = string.Empty;
                lblRgb.Text = string.Empty;
                lblHsl.Text = string.Empty;
                lblHsv.Text = string.Empty;
                lblCmyk.Text = string.Empty;
                lblLab.Text = string.Empty;
                lblMunsell.Text = string.Empty;
                lblPccs.Text = string.Empty;
                lblJis.Text = string.Empty;


            }

            // カンバス更新
            canvasView.InvalidateSurface();



        }

        /// <summary>
        /// イメージ描写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {

            if (_onCapture)
            {
                return;
            }

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            if (bitmap.Height > bitmap.Width)
            {
                // 画像とレンズのデフォルト倍率
                scaledRatioDef = (double)bitmap.Height / (double)info.Height;

                // 画像のサーズ>レンズのサーズ
                if (scaledRatioDef > 1)
                {
                    info.Width = (int)(info.Width * scaledRatioDef);
                    info.Height = bitmap.Height;
                }
            }
            else
            {
                // 画像とレンズのデフォルト倍率
                scaledRatioDef = (double)bitmap.Width / (double)info.Width;

                // 画像のサーズ>レンズのサーズ
                if (scaledRatioDef > 1)
                {
                    info.Height = (int)(info.Height * scaledRatioDef);
                    info.Width = bitmap.Width;
                }
            }

            info.Width = (int)(info.Width / scaledRatio);
            info.Height = (int)(info.Height / scaledRatio);

            if (bitmap.Width - info.Width <= 0)
            {
                defX = (bitmap.Width - info.Width) / 2;
                lastXMove = 0;
            }
            else
            {
                defX = 0;

                if (lastXMove < 0)
                {
                    lastXMove = 0;
                }
                else if (canvasView.CanvasSize.Width + Math.Abs(lastXMove) > bitmap.Width * scaledRatio / scaledRatioDef)
                {
                    lastXMove = (bitmap.Width * scaledRatio / scaledRatioDef - canvasView.CanvasSize.Width);
                }
            }

            if (bitmap.Height - info.Height <= 0)
            {
                defY = (bitmap.Height - info.Height) / 2;
                lastYMove = 0;
            }
            else
            {
                defY = 0;

                if (lastYMove < 0)
                {
                    lastYMove = 0;
                }
                else if (canvasView.CanvasSize.Height + Math.Abs(lastYMove) > bitmap.Height * scaledRatio / scaledRatioDef)
                {
                    lastYMove = (bitmap.Height * scaledRatio / scaledRatioDef - canvasView.CanvasSize.Height);
                }
            }

            SKRect mSrcRect = SKRectI.Create((int)(lastXMove * scaledRatioDef / scaledRatio) + defX, (int)(lastYMove * scaledRatioDef / scaledRatio) + defY, info.Width, info.Height);
            SKRect mDestRect = SKRectI.Create(0, 0, (int)canvasView.CanvasSize.Width, (int)canvasView.CanvasSize.Height);

            canvas.Clear();
            canvas.DrawBitmap(bitmap, mSrcRect, mDestRect);

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Xamarin.Forms.Color.White.ToSKColor(),
                StrokeWidth = 5
            };

            if (_colorChanged)
            {
                using (var bmp = new SKBitmap(info))
                {
                    IntPtr dstpixels = bmp.GetPixels();
                    ColorCode color = new ColorCode();

                    var succeed = surface.ReadPixels(info, dstpixels, info.RowBytes,
                        (int)(canvasView.CanvasSize.Width * _point.X / canvasView.Width),
                        (int)(canvasView.CanvasSize.Height * _point.Y / canvasView.Height));

                    if (Math.Abs(defX) > 0)
                    {
                        double x = Math.Abs(defX) * canvasView.Width / (canvasView.CanvasSize.Width * scaledRatioDef);
                        if (_point.X < x || _point.X > canvasView.Width - x)
                        {
                            succeed = false;
                        }
                    }

                    if (Math.Abs(defY) > 0)
                    {
                        double y = Math.Abs(defY) * canvasView.Height / (canvasView.CanvasSize.Height * scaledRatioDef);
                        if (_point.Y < y || _point.Y > canvasView.Height - y)
                        {
                            succeed = false;
                        }
                    }

                    if (succeed)
                    {
                        if (qList.Count < 10)
                        {
                            SKColor _selectedColor = bmp.GetPixel(0, 0);
                            lblRgb.Text = RGBConverter(_selectedColor);
                            string hex = ToHex(_selectedColor);
                            lblHex.Text = hex;
                            Rgb rgb = color.getRgb(hex);
                            lblHsl.Text = color.getHslString(rgb);
                            lblHsv.Text = color.getHsvString(rgb);
                            lblCmyk.Text = color.getCmykString(rgb);
                            lblLab.Text = color.getLabString(rgb);
                            lblMunsell.Text = color.getMunsellString(rgb);
                            lblPccs.Text = color.getPCCSTone(rgb);
                            lblJis.Text = JisUtil.getJisByMunsell(lblMunsell.Text);

                            paint = new SKPaint
                            {
                                Style = SKPaintStyle.Stroke,
                                Color = Xamarin.Forms.Color.White.ToSKColor(),
                                StrokeWidth = 5
                            };
                            canvas.DrawCircle((float)(canvasView.CanvasSize.Width * _point.X / canvasView.Width),
                                (float)(canvasView.CanvasSize.Height * _point.Y / canvasView.Height), (float)(20 * scaledRatio), paint);


                            ImageColorInfo colorinfo = new ImageColorInfo();
                            colorinfo.ImageID = _imageID;
                            colorinfo.HEXValue = this.lblHex.Text;
                            colorinfo.RGBValue = this.lblRgb.Text;
                            colorinfo.HSLValue = this.lblHsl.Text;
                            colorinfo.HSVValue = this.lblHsv.Text;
                            colorinfo.CMYKValue = this.lblCmyk.Text;
                            colorinfo.LABValue = this.lblLab.Text;
                            colorinfo.MUNSELLValue = this.lblMunsell.Text;
                            colorinfo.PCCSValue = this.lblPccs.Text;
                            colorinfo.JISValue = this.lblJis.Text;
                            colorinfo.XValue = (int)(_point.X);
                            colorinfo.YValue = (int)(_point.Y);

                            colorinfo.ScaledRatio = scaledRatio;
                            colorinfo.XPis = lastXMove;
                            colorinfo.YPis = lastYMove;
                            colorinfo.DefX = defX * canvasView.Width / (canvasView.CanvasSize.Width * scaledRatioDef);
                            colorinfo.DefY = defY * canvasView.Height / (canvasView.CanvasSize.Height * scaledRatioDef);

                            qList.Add(colorinfo);

                            btnColor.BackgroundColor = Xamarin.Forms.Color.FromHex(colorinfo.HEXValue);
                            lblSelected.Text = colorinfo.HEXValue;
                            updatecolorBar(qList, colorinfo.HEXValue);
                        }
                    }
                }
            }
            else
            {
                updatecolorBar(qList);
            }

            foreach (ImageColorInfo point in qList)
            {
                canvas.DrawCircle(
                    (float)((canvasView.CanvasSize.Width * (point.XValue + point.DefX - (defX * canvasView.Width / (canvasView.CanvasSize.Width * scaledRatioDef))) / canvasView.Width) * (scaledRatio / point.ScaledRatio) - lastXMove + (point.XPis * (scaledRatio / point.ScaledRatio))),
                    (float)((canvasView.CanvasSize.Height * (point.YValue + point.DefY - (defY * canvasView.Height / (canvasView.CanvasSize.Height * scaledRatioDef))) / canvasView.Height) * (scaledRatio / point.ScaledRatio) - lastYMove + (point.YPis * (scaledRatio / point.ScaledRatio))),
                    (float)(20 * scaledRatio), paint);
            }

            _colorChanged = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private string ToHex(SKColor color)
        {
            return String.Format("#{0}{1}{2}"
                , color.Red.ToString("X").Length == 1 ? String.Format("0{0}", color.Red.ToString("X")) : color.Red.ToString("X")
                , color.Green.ToString("X").Length == 1 ? String.Format("0{0}", color.Green.ToString("X")) : color.Green.ToString("X")
                , color.Blue.ToString("X").Length == 1 ? String.Format("0{0}", color.Blue.ToString("X")) : color.Blue.ToString("X"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private String HexConverter(SKColor c)
        {
            String rtn = String.Empty;
            try
            {
                rtn = "#" + c.Red.ToString("X2") + c.Green.ToString("X2") + c.Blue.ToString("X2");
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return rtn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static String RGBConverter(SKColor c)
        {
            String rtn = String.Empty;
            try
            {
                rtn = c.Red.ToString() + "," + c.Green.ToString() + "," + c.Blue.ToString();
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return rtn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            try
            {
                var point = args.Location[0];
                var id = args.Id;
                switch (args.Type)
                {
                    case TouchActionType.Pressed:
                        // 2指タッチ
                        if (args.PointerCount == 2)
                        {
                            touchId = args.Id;
                            pressPoint1 = args.Location[0];
                            pressPoint2 = args.Location[1];
                            pressTime = DateTime.Now;
                            lastFingerDis = distanceBetweenFingers(args);
                            _colorChanged = false;
                        }
                        else
                        {
                            _colorChanged = true;
                        }
                        break;
                    case TouchActionType.Moved:
                        if (args.PointerCount == 2)
                        {
                            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
                            TimeSpan ts2 = new TimeSpan(pressTime.Ticks);
                            TimeSpan ts3 = ts1.Subtract(ts2);

                            double xMove1 = pressPoint1.X - args.Location[0].X;
                            double yMove1 = pressPoint1.Y - args.Location[0].Y;
                            double xMove2 = pressPoint2.X - args.Location[1].X;
                            double yMove2 = pressPoint2.Y - args.Location[1].Y;

                            if (!distanceBetweenFingers(xMove1, yMove1, xMove2, yMove2))
                            {
                                _colorChanged = false;
                                lastXMove = xMove1 + lastXMoveBak;
                                lastYMove = yMove1 + lastYMoveBak;
                                canvasView.InvalidateSurface();
                            }
                            else
                            {
                                iTime += (long)float.Parse(ts3.TotalMilliseconds.ToString());

                                if (iTime > 2000)
                                {
                                    double fingerDis = distanceBetweenFingers(args);
                                    if (fingerDis > lastFingerDis)
                                    {
                                        _colorChanged = false;

                                        if (scaledRatio == double.Parse(dicConfig["scaledRatio1"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio2"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio2"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio3"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio3"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio4"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio4"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio5"]);
                                        }
                                    }
                                    else
                                    {
                                        if (scaledRatio == double.Parse(dicConfig["scaledRatio5"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio4"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio4"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio3"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio3"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio2"]);
                                        }
                                        else if (scaledRatio == double.Parse(dicConfig["scaledRatio2"]))
                                        {
                                            scaledRatio = double.Parse(dicConfig["scaledRatio1"]);
                                        }

                                        _colorChanged = false;
                                    }
                                    iTime = 0;
                                    canvasView.InvalidateSurface();
                                }
                            }
                        }
                        break;
                    case TouchActionType.Released:

                        if (args.PointerCount == 1)
                        {
                            if (_colorChanged)
                            {
                                if (point != _point)
                                {
                                    _colorChanged = true;
                                    _point = point;

                                    canvasView.InvalidateSurface();
                                }
                                else
                                {
                                    _colorChanged = false;
                                }
                            }
                            lastXMoveBak = lastXMove;
                            lastYMoveBak = lastYMove;
                        }
                        break;
                    case TouchActionType.Cancelled:
                        lastXMoveBak = lastXMove;
                        lastYMoveBak = lastYMove;
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// ズーム判断する
        /// </summary>
        /// <param name="lst"></param>
        private Boolean distanceBetweenFingers(double xMove1, double yMove1, double xMove2, double yMove2)
        {

            bool disX = (xMove1 >= 0 && 0 >= xMove2) || (xMove2 >= 0 && 0 >= xMove1);
            bool disY = (yMove1 >= 0 && 0 >= yMove2) || (yMove2 >= 0 && 0 >= yMove1);
            return disX && disY;
        }

        private double distanceBetweenFingers(TouchActionEventArgs args)
        {
            double disX = Math.Abs(args.Location[0].X - args.Location[1].X);
            double disY = Math.Abs(args.Location[0].Y - args.Location[1].Y);
            return Math.Sqrt(disX * disX + disY * disY);
        }

        /// <summary>
        /// 選択した色リストを更新する
        /// </summary>
        /// <param name="lst"></param>
        private void updatecolorBar(List<ImageColorInfo> lst, string pickColor = "")
        {
            pickColorList.Children.Clear();

            int colorcnt = lst.Count;
            double heightValue = pickColorList.Width / 10;

            ColorCode colorcd = new ColorCode();

            foreach (ImageColorInfo color in lst)
            {

                BoxView box = new BoxView();
                box.BackgroundColor = Xamarin.Forms.Color.FromHex(color.HEXValue);
                box.Color = Xamarin.Forms.Color.FromHex(color.HEXValue);
                double widthHeight = pickColorList.Width / colorcnt;
                box.WidthRequest = widthHeight;
                box.HeightRequest = heightValue;

                if (lblSelected.Text.Equals(color.HEXValue))
                {
                    box.HeightRequest = heightValue * 2;
                    box.Margin = new Thickness(0, 0, 0, -heightValue);
                }
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) =>
                {

                    foreach (BoxView newBox in pickColorList.Children)
                    {
                        newBox.HeightRequest = heightValue;
                        newBox.Margin = 0;
                    }
                    BoxView boxview = (BoxView)s;
                    boxview.HeightRequest = heightValue * 2;
                    boxview.Margin = new Thickness(0, 0, 0, -heightValue);
                    btnColor.BackgroundColor = boxview.Color;

                    lblSelected.Text = ToHex(boxview.BackgroundColor.ToSKColor());

                    string hex = ToHex(boxview.BackgroundColor.ToSKColor());
                    lblHex.Text = hex;
                    Rgb rgb = colorcd.getRgb(hex);
                    lblRgb.Text = colorcd.getRgbString(rgb);
                    lblHsl.Text = colorcd.getHslString(rgb);
                    lblHsv.Text = colorcd.getHsvString(rgb);
                    lblCmyk.Text = colorcd.getCmykString(rgb);
                    lblLab.Text = colorcd.getLabString(rgb);
                    lblMunsell.Text = colorcd.getMunsellString(rgb);
                    lblPccs.Text = colorcd.getPCCSTone(rgb);
                    lblJis.Text = JisUtil.getJisByMunsell(lblMunsell.Text);
                };
                box.GestureRecognizers.Add(tapGestureRecognizer);

                pickColorList.Children.Add(box);

            }
        }


        private async void SetColorbar(int imageID)
        {
            if (_imageID != -1)
            {
                List<Model.ImageColorInfo> lst = await SqliteUtil.Current.QueryImageColorInfoByImageID(_imageID);
                foreach (ImageColorInfo color in lst)
                {
                    BoxView box = new BoxView();
                    box.Color = Xamarin.Forms.Color.FromHex(color.HEXValue);
                    box.CornerRadius = new CornerRadius(10);
                    pickColorList.Children.Add(box);
                }

                if (lst.Count > 0)
                {
                    ColorCode colorcd = new ColorCode();
                    string hex = lst[lst.Count - 1].HEXValue;
                    btnColor.BackgroundColor = Xamarin.Forms.Color.FromHex(hex);
                    lblSelected.Text = hex;
                    lblHex.Text = hex;

                    Rgb rgb = colorcd.getRgb(hex);
                    lblRgb.Text = colorcd.getRgbString(rgb);
                    lblHsl.Text = colorcd.getHslString(rgb);
                    lblHsv.Text = colorcd.getHsvString(rgb);
                    lblCmyk.Text = colorcd.getCmykString(rgb);
                    lblLab.Text = colorcd.getLabString(rgb);
                    lblMunsell.Text = colorcd.getMunsellString(rgb);
                    lblPccs.Text = colorcd.getPCCSTone(rgb);
                    lblJis.Text = JisUtil.getJisByMunsell(lblMunsell.Text);
                }

            }
        }

        public ImageDetail(MediaFile file, int imageID) : this()
        {
            setImageDetail(file, imageID);

            //按照屏幕的宽度和图片实际宽度，计算显示比率，再计算同比率显示的高度
            //int dispWidthZoom = bitmap.Width / App.ScreenWidth;
            //int dispHeigh = 1;
            //if (dispWidthZoom >= 1)
            //{
            //    dispHeigh = bitmap.Height / dispWidthZoom;
            //    if (dispHeigh >= App.ScreenHeight * 0.75)
            //        dispHeigh = (int)(App.ScreenHeight * 0.75);
            //    gridCanvas.HeightRequest = dispHeigh;
            //}            
        }

        public void setImageDetail(MediaFile file, int imageID)
        {
            seletedimg = file.GetStream();

            bitmap = SKBitmap.Decode(seletedimg);

            TitleName = "";

            //データベースからカラートンを取得する
            _imageID = imageID;
            if (_imageID != -1)
            {
                SetColorbar(imageID);
            }
            inputtype = 1;
            currentFile = file;

            _fullFilePath = file.Path;
        }


        public ImageDetail(byte[] byteArray, int imageID, string filepath) : this()
        {
            setImageDetail(byteArray, imageID, filepath);
            //按照屏幕的宽度和图片实际宽度，计算显示比率，再计算同比率显示的高度
            //int dispWidthZoom = bitmap.Width / App.ScreenWidth;
            //int dispHeigh = 1;
            //if (dispWidthZoom >= 1)
            //{
            //    dispHeigh = bitmap.Height / dispWidthZoom;
            //    if (dispHeigh >= App.ScreenHeight * 0.75)
            //        dispHeigh = (int)(App.ScreenHeight * 0.75);
            //    gridCanvas.HeightRequest = dispHeigh;
            //}            
        }

        public void setImageDetail(byte[] byteArray, int imageID, string filepath)
        {
            seletedimg = new MemoryStream(byteArray);
            bitmap = SKBitmap.Decode(seletedimg);
            TitleName = "";
            //データベースからカラートンを取得する
            _imageID = imageID;
            if (_imageID != -1)
            {
                SetColorbar(imageID);
            }
            inputtype = 2;
            CurrentbyteArray = new byte[byteArray.Length];
            CurrentbyteArray = byteArray;

            _fullFilePath = filepath;
        }

        public ImageDetail(byte[] byteArray, List<ImageColorInfo> inputList, string filepath, String name) : this()
        {
            setImageDetail(byteArray, inputList, filepath, name);
        }

        public void setImageDetail(byte[] byteArray, List<ImageColorInfo> inputList, string filepath, String name)
        {
            seletedimg = new MemoryStream(byteArray);
            bitmap = SKBitmap.Decode(seletedimg);

            qList = inputList;
            inputtype = 3;
            CurrentbyteArray = new byte[byteArray.Length];
            CurrentbyteArray = byteArray;

            _fullFilePath = filepath;

            TitleName = name;
        }


        private void btn_Return(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new MainPage());
        }


        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string strHex = lblSelected.Text;
            ColorCode colorcd = new ColorCode();

            foreach (ImageColorInfo info in qList)
            {
                if (info.HEXValue.Equals(strHex))
                {
                    qList.Remove(info);
                    break;
                }
            }
            _point = new Xamarin.Forms.Point(0.0, 0.0);
            lastXMove = lastXMoveBak;
            lastYMove = lastYMoveBak;
            if (qList.Count > 0)
            {

                string hex = qList[qList.Count - 1].HEXValue;
                btnColor.BackgroundColor = Xamarin.Forms.Color.FromHex(hex);
                lblSelected.Text = hex;
                lblHex.Text = hex;
                Rgb rgb = colorcd.getRgb(hex);
                lblRgb.Text = colorcd.getRgbString(rgb);
                lblHsl.Text = colorcd.getHslString(rgb);
                lblHsv.Text = colorcd.getHsvString(rgb);
                lblCmyk.Text = colorcd.getCmykString(rgb);
                lblLab.Text = colorcd.getLabString(rgb);
                lblMunsell.Text = colorcd.getMunsellString(rgb);
                lblPccs.Text = colorcd.getPCCSTone(rgb);
                lblJis.Text = JisUtil.getJisByMunsell(lblMunsell.Text);
            }
            else
            {
                btnColor.BackgroundColor = Xamarin.Forms.Color.White;
                lblSelected.Text = string.Empty;
                lblHex.Text = string.Empty;
                lblRgb.Text = string.Empty;
                lblHsl.Text = string.Empty;
                lblHsv.Text = string.Empty;
                lblCmyk.Text = string.Empty;
                lblLab.Text = string.Empty;
                lblMunsell.Text = string.Empty;
                lblPccs.Text = string.Empty;
                lblJis.Text = string.Empty;
            }

            // カンバス更新
            canvasView.InvalidateSurface();
        }

        private async void btn_SaveImageInfo(object sender, EventArgs e)
        {
            if (seletedimg == null)
                return;

            string docDir = DirectoryUtil.Current.GetDir();

            int ret = -1;

            if (_imageID == -1)
            {
                using (SKImage image = SKImage.FromBitmap(bitmap))
                {
                    SKData data = image.Encode();
                    DateTime dt = DateTime.Now;
                    string filename = String.Format("{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                    dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                    Model.IamgeSaveInfo imageinfo = new IamgeSaveInfo();

                    imageinfo.CateLogId = -1;
                    //_ImagePath = Path.Combine(Path.Combine(docDir, "Iroai"), filename);
                    imageinfo.ImagePath = _fullFilePath;
                    imageinfo.Createdatetime = DateTime.Now;
                    imageinfo.Imagebuffer = Convert.ToBase64String(data.ToArray());

                    ret = await SqliteUtil.Current.InsertImageInfo(imageinfo);

                    if (ret == 1)
                    {
                        Model.IamgeSaveInfo savedInfo = await SqliteUtil.Current.QueryImageByFileName(imageinfo.ImagePath);

                        if (savedInfo == null)
                        {
                            await DisplayAlert("Iroai", "Save Color infomation error. Sorry!", "OK");
                        }
                        else
                        {
                            _imageID = savedInfo.ImageID;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Iroai", "Save Color infomation error. Sorry!", "OK");
                        return;
                    }
                }
            }

            // save color info
            ImageColorInfo colorinfo = new ImageColorInfo();
            colorinfo.ImageID = _imageID;
            colorinfo.HEXValue = this.lblHex.Text;
            colorinfo.RGBValue = this.lblRgb.Text;
            colorinfo.HSLValue = this.lblHsl.Text;
            colorinfo.HSVValue = this.lblHsv.Text;
            colorinfo.CMYKValue = this.lblCmyk.Text;
            colorinfo.LABValue = this.lblLab.Text;
            colorinfo.MUNSELLValue = this.lblMunsell.Text;
            colorinfo.PCCSValue = this.lblPccs.Text;
            colorinfo.JISValue = this.lblJis.Text;
            colorinfo.XValue = (int)_point.X;

            colorinfo.YValue = (int)_point.Y;
            ret = await SqliteUtil.Current.InsertImageColorInfo(colorinfo);
            if (ret != 1)
            {
                await DisplayAlert("Iroai", "Save Color infomation error. Sorry!", "OK");
                return;
            }
            else
            {
                //await Navigation.PushModalAsync(new MainStoreList(_imageID));
            }
        }

        private async void btnRead_Clicked(object sender, EventArgs e)
        {
            //DependencyService.Get<IAdInterstitial>().ShowAd();
            //Controls.Get<IAdInterstitial>().ShowAd();
            //AdMobBanner.ShowAd();
            await Navigation.PushPopupAsync(new LoadMenu(_fullFilePath));
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (qList.Count == 0)
            {
                await DisplayAlert("", "カラーを選択して下さい", "確定");
                return;
            }

            if (seletedimg == null)
                return;

            PromptResult promptResult = await UserDialogs.Instance.PromptAsync("保存タイトル名", "ｉｒｏａｉリスト", "保存", "キャンセル", "", InputType.Name);
            if (promptResult.Ok)
            {

                string sTitle = promptResult.Text;

                List<CataLogInfo> catologInfoList = await SqliteUtil.Current.QueryCataLogsByTitle(sTitle);
                if (catologInfoList.Count > 0)
                {
                    await DisplayAlert("", "保存名称が重複しています。", "確定");
                    return;
                }

                arrayList.Clear();
                _listinfo.Clear();
                arrayList.AddRange(qList);

                lock (arrayList.SyncRoot)
                {
                    for (int k = 0; k < arrayList.Count; k++)
                    {
                        _listinfo.Add(arrayList[k] as ImageColorInfo);
                    }
                }

                string docDir = DirectoryUtil.Current.GetDir();

                using (SKImage image = SKImage.FromBitmap(bitmap))
                {
                    using (UserDialogs.Instance.Loading("保存中", null, null, true, MaskType.Black))
                    {
                        await Task.Delay(2000);
                        SKData data = image.Encode();
                        DateTime dt = DateTime.Now;
                        string filename = String.Format("{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                        dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                        //_ImagePath = Path.Combine(Path.Combine(docDir, "Iroai"), filename);

                        if (!string.IsNullOrWhiteSpace(sTitle))
                        {
                            OnSaveTitle(sTitle, data);
                        }
                        else
                        {
                            await DisplayAlert("", "保存名称を入力して下さい。", "確定");
                        }
                    }
                }
            }
        }

        public async void OnSaveTitle(string title, SKData data)
        {
            try
            {
                int maxmaxImageId = -1;

                //search the max imageId from table [ImageColorInfo] 
                List<ImageColorInfo> ImageIdList = await SqliteUtil.Current.QueryImageColorInfo();
                if (ImageIdList == null)
                    return;

                if (ImageIdList.Count > 0)
                {
                    maxmaxImageId = ImageIdList[ImageIdList.Count - 1].ImageID;
                }

                maxmaxImageId++;

                foreach (var imageColorItem in _listinfo)
                {
                    ImageColorInfo imageColorInfo = new ImageColorInfo();
                    imageColorInfo.ImageID = maxmaxImageId;
                    imageColorInfo.HEXValue = imageColorItem.HEXValue;
                    imageColorInfo.RGBValue = imageColorItem.RGBValue;
                    imageColorInfo.HSLValue = imageColorItem.HSLValue;
                    imageColorInfo.HSVValue = imageColorItem.HSVValue;
                    imageColorInfo.CMYKValue = imageColorItem.CMYKValue;
                    imageColorInfo.LABValue = imageColorItem.LABValue;
                    imageColorInfo.MUNSELLValue = imageColorItem.MUNSELLValue;
                    imageColorInfo.PCCSValue = imageColorItem.PCCSValue;
                    imageColorInfo.XValue = imageColorItem.XValue;
                    imageColorInfo.YValue = imageColorItem.YValue;
                    imageColorInfo.ScaledRatio = imageColorItem.ScaledRatio;
                    imageColorInfo.XPis = imageColorItem.XPis;
                    imageColorInfo.YPis = imageColorItem.YPis;
                    imageColorInfo.DefX = imageColorItem.DefX;
                    imageColorInfo.DefY = imageColorItem.DefY;
                    int retImageColorInfo = await SqliteUtil.Current.InsertImageColorInfo(imageColorInfo);
                }

                //insert into table [IamgeSaveInfo] 
                IamgeSaveInfo imageinfo = new IamgeSaveInfo();
                imageinfo.ImageID = maxmaxImageId;
                imageinfo.ImagePath = _fullFilePath;
                imageinfo.Imagebuffer = Convert.ToBase64String(data.ToArray());
                imageinfo.Createdatetime = DateTime.Now;
                int retIamgeSaveInfo = await SqliteUtil.Current.InsertImageInfo(imageinfo);

                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByImageID(maxmaxImageId);
                var CateLogIdGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.CateLogId })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                List<CataLogInfo> cataList = await SqliteUtil.Current.QueryCataLogInfoByCateLogId(CateLogIdGroupList[CateLogIdGroupList.Count - 1].Keys.CateLogId);
                var CateLogIdTitleGroupList = cataList
                    .GroupBy(x => new { x.Title })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                CataLogInfo catalog = new CataLogInfo();
                catalog.CateLogId = CateLogIdGroupList[CateLogIdGroupList.Count - 1].Keys.CateLogId;
                catalog.Folder = "";
                catalog.Title = title;
                catalog.CreateDatetime = DateTime.Now;
                catalog.Description = "風景";

                int ret = await SqliteUtil.Current.InsertCataLog(catalog);
                if (ret == 1)
                {
                    TitleName = title;
                    await DisplayAlert("", "保存しました。", "OK");
                    //DependencyService.Get<IAdmobView>().Show();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void btnHome_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new MainPage());
            bool check;
            check = await  DisplayAlert("", "保存前の色情報は破棄されます。\nトップへ戻りますか?", "はい", "キャンセル");
            if (check) {
                App.Current.MainPage = App.mainPage;
            }
            
        }

        public event EventHandler<EventArgs> OperationCompeleted;

        private async void btnOpen_Clicked(object sender, EventArgs e)
        {
            List<ImageColorInfo> ImageColorList = await SqliteUtil.Current.QueryImageColorInfo();

            if (ImageColorList == null)
                return;

            if (ImageColorList.Count == 0)
            {
                await DisplayAlert("", "保存データはありません。", "OK");
            }
            else
            {
                if (inputtype == 1)
                {
                    App.srcPage = new ImageDetail(currentFile, _imageID);
                }
                else if (inputtype == 2)
                {
                    App.srcPage = new ImageDetail(CurrentbyteArray, _imageID, _fullFilePath);
                }
                else if (inputtype == 3)
                {
                    App.srcPage = new ImageDetail(CurrentbyteArray, qList, _fullFilePath, "");
                }

                StoreListDetail storeListDetail = new StoreListDetail("ｉｒｏａｉ画面");
                // Subscribe to the event when things are updated
                storeListDetail.OperationCompeleted += StoreListDetail_OperationCompleted;
                await Navigation.PushModalAsync(storeListDetail);
            }
        }

        private async void StoreListDetail_OperationCompleted(object sender, EventArgs e)
        {
            // Unsubscribe to the event to prevent memory leak
            (sender as StoreListDetail).OperationCompeleted -= StoreListDetail_OperationCompleted;

            EventArgsEx ex = (EventArgsEx)e;

            if (ex.Filepath != null)
            {
                setImageDetail(ex.ByteArray, ex.InputList, ex.Filepath, ex.TitleName);

            }


        }


        private void btnCommon_Clicked(object sender, EventArgs e)
        {

            _onCapture = true;

            var screenshotData = DependencyService.Get<IScreenshotService>().Capture(
                    DateTime.Now.ToString("yyyyMMddHHmmss_")
                    + (Device.RuntimePlatform == Device.iOS ? "capture.png" : "capture.jpg"));

            _onCapture = false;

            DependencyService.Get<IShare>().Show("Share ", "Share My Picture!", screenshotData);

        }

        private async void btnHelp_Clicked(object sender, EventArgs e)
        {
            // bool checkalert;
            // checkalert = await DisplayAlert("", "アプリから移動します。\nよろしいですか？", "はい", "キャンセル");
            // if (checkalert) {
                await Navigation.PushPopupAsync(new TopMenu());
            // }
            
        }
    }
}