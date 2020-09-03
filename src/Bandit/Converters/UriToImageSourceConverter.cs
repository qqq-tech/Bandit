using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bandit.Converters
{
    [ValueConversion(typeof(Uri), typeof(ImageSource))]
    public class UriToImageSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (targetType != typeof(ImageSource))
            {
                throw new InvalidOperationException("The target must be a ImageSource!");
            }

            byte[] buffer;

            using (WebClient webClient = new WebClient())
            {
                buffer = webClient.DownloadData((Uri)value);
            }

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();

                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
