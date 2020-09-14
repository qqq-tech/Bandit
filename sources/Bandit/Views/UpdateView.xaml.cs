using Bandit.Models;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace Bandit.Views
{
    /// <summary>
    /// HistoryView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UpdateView : Window
    {
        public UpdateView()
        {
            InitializeComponent();
            InitializeHistory();
        }

        public void InitializeHistory()
        {
            string history = GetWebContents(Settings.URL_BANDIT_HISTORY_LATEST);
            HistoryTextBlock.Text = history;
        }

        public string GetWebContents(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                // 스트림을 읽어와서 문자열로 저장한다.
                string versionString = reader.ReadToEnd();

                // 버전 값을 파싱한 후 반환한다.
                return versionString;
            }
        }

        private void OnDownloadButtonClick(object sender, RoutedEventArgs e)
        {
            string url = string.Format(Settings.URL_BANDIT_RELEASE, GetWebContents(Settings.URL_BANDIT_LATEST_VERSION));
            Process.Start(url);
        }
    }
}
