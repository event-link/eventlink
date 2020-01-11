using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace EventLink.API.Util
{
    public interface IFTPClient
    {
        void UploadFile(string fileName, string fileData);
    }

    public class FTPClientException : Exception
    {
        public FTPClientException() { }
        public FTPClientException(string message) : base(message) { }
        public FTPClientException(string message, Exception inner) : base(message, inner) { }
    }

    public class FTPClient : IFTPClient
    {

        const string ftpClientUrl = "ftp://bak.eventlink.ml//files/";

        private static readonly Lazy<IFTPClient> instance =
            new Lazy<IFTPClient>(() => new FTPClient());

        public static IFTPClient Instance => instance.Value;

        private FTPClient()
        {

        }

        public void UploadFile(string fileName, string fileData)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var fileDataBytes = Convert.FromBase64String(fileData);

                    Image image;
                    using (MemoryStream ms = new MemoryStream(fileDataBytes))
                    {
                        image = Image.FromStream(ms);
                    }

                    var newImage = ResizeImage(image, 200, 200);

                    var newFileDataBytes = ImageToByteArray(newImage);

                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSetting["FTP:Username"], ConfigurationManager.AppSetting["FTP:Password"]);
                    client.UploadData(ftpClientUrl + fileName, newFileDataBytes);
                }
            }
            catch (Exception e)
            {
                throw new FTPClientException(e.Message, e);
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private Image ResizeImage(Image source, int width, int height)
        {
            Image result = null;

            try
            {
                if (source.Width != width || source.Height != height)
                {
                    // Resize image
                    float sourceRatio = (float)source.Width / source.Height;

                    using (var target = new Bitmap(width, height))
                    {
                        using (var g = System.Drawing.Graphics.FromImage(target))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;

                            // Scaling
                            float scaling;
                            float scalingY = (float)source.Height / height;
                            float scalingX = (float)source.Width / width;
                            if (scalingX < scalingY) scaling = scalingX; else scaling = scalingY;

                            int newWidth = (int)(source.Width / scaling);
                            int newHeight = (int)(source.Height / scaling);

                            // Correct float to int rounding
                            if (newWidth < width) newWidth = width;
                            if (newHeight < height) newHeight = height;

                            // See if image needs to be cropped
                            int shiftX = 0;
                            int shiftY = 0;

                            if (newWidth > width)
                            {
                                shiftX = (newWidth - width) / 2;
                            }

                            if (newHeight > height)
                            {
                                shiftY = (newHeight - height) / 2;
                            }

                            // Draw image
                            g.DrawImage(source, -shiftX, -shiftY, newWidth, newHeight);
                        }
                        result = (Image)target.Clone();
                    }
                }
                else
                {
                    result = (Image)source.Clone();
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

    }

}