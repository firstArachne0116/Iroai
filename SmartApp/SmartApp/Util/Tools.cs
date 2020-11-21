using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xamarin.Forms;

namespace SmartApp.Util
{
    public class Tools
    {
        public static Xamarin.Forms.Image bytesToImage(byte[] data)
        {
            var image = new Xamarin.Forms.Image();
            image.Source = ImageSource.FromStream(() => new MemoryStream(data));
            return image;
        }


        public static byte[] StreamTobytes(Stream stream)
        {
            byte[] result;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                stream.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            return result;
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }

        //public static byte[] base64stringToByteArray(string base64string)
        //{
        //    byte[] imageBytes = Convert.FromBase64String(base64string);
        //    //读入MemoryStream对象
        //    MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //    memoryStream.Write(imageBytes, 0, imageBytes.Length);
        //    //转成图片
        //    Image image = Image.FromStream(memoryStream);

        //}
    }
}
