using Bandit.Models;
using HtmlAgilityPack;
using Ionic.Zip;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bandit.Utilities
{
    internal class DriverUtility
    {
        #region ::Version-Related::

        public string GetVersion(string driverPath)
        {
            if (!File.Exists(driverPath))
                return null;

            IWebDriver driver = new ChromeDriver(driverPath);
            ICapabilities capabilities = ((RemoteWebDriver)driver).Capabilities;
            return (string)(capabilities.GetCapability("chrome") as Dictionary<string, object>)["chromedriverVersion"];
        }

        public bool IsExistsVersion(Version version)
        {
            try
            {
                bool result = false;
                string url = string.Format(Settings.URL_CHROMEDRIVERS_DOWNLOAD, version.ToString());

                // 응답 요청을 작성한다.
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";

                // 서버로 요청을 발송하고 응답을 대기한다.
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        result = true;

                    response.Close();
                    response.Dispose();
                }

                return result;
            }
            catch
            {
                //Any exception will returns false. return false;
                return false;
            }
        }

        internal List<Version> GetVersionList()
        {
            WebRequest webRequest = WebRequest.Create(Settings.URL_CHROMEDRIVERS_DOWNLOAD_LIST);
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                // 스트림을 읽어와서 문자열로 저장한다.
                string xml = reader.ReadToEnd();

                // 새로운 XML 문서를 생성한다.
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                
                XmlElement rootElement = document.DocumentElement;
                
                // 루트 엘리먼트에서 모든 Key 엘리먼트들의 목록을 불러온다.
                XmlNodeList nodes = rootElement.GetElementsByTagName("Key");

                if (nodes == null || nodes.Count <= 0)
                    return new List<Version>();

                List<Version> versionList = new List<Version>();

                // Win32 환경의 배포 파일을 포함하는 경우라면 리스트에 등록한다.
                foreach (XmlNode node in nodes)
                {
                    if (node.InnerText.Contains("chromedriver_win32.zip"))
                    {
                        string versionString = node.InnerText.Replace("/chromedriver_win32.zip", "");
                        Version version = new Version(versionString);
                        versionList.Add(version);
                    }
                }

                return versionList;
            }
        }

        internal Version GetLatestVersion()
        {
            WebRequest webRequest = WebRequest.Create(Settings.URL_CHROMEDRIVERS_LATEST);
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                // 스트림을 읽어와서 문자열로 저장한다.
                string versionString = reader.ReadToEnd();

                // 버전 값을 파싱한 후 반환한다.
                return new Version(versionString);
            }
        }

        #endregion

        #region ::Downloading-Related::

        public delegate void DownloadProgressChangedEventHandler(double progress);

        public event AsyncCompletedEventHandler DownloadDriverCompleted;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        internal void DownloadDriverAsync(Version version, string filePath)
        {
            string url = string.Format(Settings.URL_CHROMEDRIVERS_DOWNLOAD, version.ToString());

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += OnDownloadProgressChanged;
                webClient.DownloadFileCompleted += OnDownloadCompleted;

                try
                {
                    webClient.DownloadFileAsync(new Uri(url), filePath);
                }
                catch (Exception ex)
                {
                    Reports.Instance.AddReport(Entities.ReportType.Warning, $"{ex.Message} {ex.StackTrace}");
                }
                finally
                {
                    webClient.Dispose();
                }
            }
        }

        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(e.BytesReceived / e.TotalBytesToReceive * 100);
        }

        void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadDriverCompleted?.Invoke(this, e);
        }

        #endregion

        #region ::Decompressing-Related::

        public delegate void DecompressProgressChangedEventHandler(double progress);

        public event DecompressProgressChangedEventHandler DecompressProgressChanged;

        public void DecompressFile(string filePath, string targetDirectory)
        {
            if (!File.Exists(filePath))
                return;

            try
            {
                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                using (ZipFile zip = new ZipFile(filePath))
                {
                    /// 압축 해제 진행률 표시를 위한 이벤트 핸들러 등록.
                    zip.ReadProgress += new EventHandler<ReadProgressEventArgs>(OnDecompressProgressChanged);
                    zip.ExtractAll(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Reports.Instance.AddReport(Entities.ReportType.Warning, $"패키지 압축 해제 중 오류가 발생했습니다. 다시 시도해주시기 바랍니다. {ex.Message} {ex.StackTrace}");
            }
        }

        private void OnDecompressProgressChanged(object sender, ReadProgressEventArgs e)
        {
            DecompressProgressChanged?.Invoke(e.BytesTransferred / e.TotalBytesToTransfer * 100);
        }

        #endregion
    }
}
