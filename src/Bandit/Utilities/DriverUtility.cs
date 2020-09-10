using Bandit.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Xml;

namespace Bandit.Utilities
{
    /// <summary>
    /// 드라이버와 관련된 기능을 제공하는 클래스입니다.
    /// </summary>
    public class DriverUtility
    {
        #region ::Version-Related::

        /// <summary>
        /// 지정된 버전이 존재하는지에 대한 여부를 반환합니다.
        /// </summary>
        /// <param name="version">존재 여부를 확인할 버전입니다.</param>
        /// <returns>해당 버전의 존재 여부.</returns>
        public bool IsExistsVersion(Version version)
        {
            try
            {
                bool result = false;

                string url = string.Format(Settings.URL_CHROMEDRIVERS_DOWNLOAD, version.ToString());

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";

                // 서버로 요청을 발송하고 응답을 대기한다.
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        result = true;
                    }

                    response.Close();
                    response.Dispose();
                }

                return result;
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

        /// <summary>
        /// 사용 가능한 버전 목록을 가져옵니다.
        /// </summary>
        /// <returns>사용 가능한 버전 목록.</returns>
        public List<Version> GetVersions()
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
                {
                    return new List<Version>();
                }

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

        /// <summary>
        /// 현재 최신 버전을 가져옵니다.
        /// </summary>
        /// <returns>현재 최신 버전.</returns>
        public Version GetLatestVersion()
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

        /// <summary>
        /// 다운로드 진행률 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="progress">현재 다운로드 진행률입니다.</param>
        public delegate void DownloadProgressChangedEventHandler(double progress);

        /// <summary>
        /// 드라이버 다운로드가 완료되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public event AsyncCompletedEventHandler DownloadDriverCompleted;

        /// <summary>
        /// 드라이버 다운로드 진행률이 변경되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// 드라이버를 비동기 방식으로 다운로드합니다. 이 메소드는 호출 스레드를 차단하지 않습니다.
        /// </summary>
        /// <param name="version">다운로드할 드라이버의 버전입니다.</param>
        /// <param name="filePath">드라이버 패키지가 저장될 경로입니다.</param>
        public void DownloadDriverAsync(Version version, string filePath)
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
                    Reports.Instance.AddReport(Entities.ReportType.Warning, $"패키지 다운로드 중 오류가 발생했습니다. 다시 시도해주시기 바랍니다. {ex.Message} {ex.StackTrace}");
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

        /// <summary>
        /// 압축 해제 진행률 변경 이벤트 핸들러입니다.
        /// </summary>
        /// <param name="progress">현재 압축 해제 진행률입니다.</param>
        public delegate void DecompressProgressChangedEventHandler(double progress);

        /// <summary>
        /// 압축 해제 진행률이 변경되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public event DecompressProgressChangedEventHandler DecompressProgressChanged;

        /// <summary>
        /// 지정된 패키지의 압축을 해제합니다.
        /// </summary>
        /// <param name="filePath">압축을 풀 패키지의 경로입니다.</param>
        /// <param name="targetDirectory">압축 해제된 파일이 저장될 경로입니다.</param>
        public void DecompressFile(string filePath, string targetDirectory)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using (ZipFile zip = new ZipFile(filePath))
                {
                    /// 압축 해제 진행률 표시를 위한 이벤트 핸들러 등록.
                    zip.ReadProgress += OnDecompressProgressChanged;
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
