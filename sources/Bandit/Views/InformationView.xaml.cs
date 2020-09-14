using Bandit.Models;
using Bandit.Utilities;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bandit.Views
{
    /// <summary>
    /// InformationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InformationView : Window
    {
        public InformationView()
        {
            InitializeComponent();
            InitializeText();
            InitializeImage();
        }

        private void SetRichText(RichTextBox rtb, string document)
        {
            var documentBytes = Encoding.UTF8.GetBytes(document);
            using (var reader = new MemoryStream(documentBytes))
            {
                reader.Position = 0;
                rtb.SelectAll();
                rtb.Selection.Load(reader, DataFormats.Rtf);
            }
        }

        public BitmapImage ConvertToBitmapImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void InitializeText()
        {
            if (!File.Exists(Settings.PATH_VERSION))
            {
                VersionTextBlock.Text = "버전 정보를 불러올 수 없습니다.";
            }
            else
            {
                VersionTextBlock.Text = string.Format("release v{0}", FileUtility.ReadTextFile(Settings.PATH_VERSION, Encoding.UTF8));
            }

            SetRichText(OpensourceTextBox, Properties.Resources.open_source_software_notice);
        }

        private void InitializeImage()
        {
            BanditImage.Source = ConvertToBitmapImage(Properties.Resources.bandit_image);
        }
    }
}
