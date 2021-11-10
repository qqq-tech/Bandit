using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace Bandit
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private bool IsInternetConnected()
        {
            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try
            {
                // NCSI 테스트 링크 접속 확인.
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT)
                {
                    return false;
                }

                // NCSI DNS 주소 확인.
                var dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() < 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        private string GetWebContents(string url)
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

        protected override void OnStartup(StartupEventArgs e)
        {
            // 처리되지 않은 예외 발생 시.
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledExceptionOccurs;

            // 인터넷 사용 가능 여부 확인.
            if (!IsInternetConnected())
            {
                MessageBox.Show("인터넷에 접속할 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            // 프로세스 중복 실행 여부 확인.
            Process[] procs = Process.GetProcessesByName("Bandit");
            if (procs.Length > 1)
            {
                MessageBox.Show("프로그램이 이미 실행중입니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            if (!File.Exists(Settings.PATH_SETTINGS))
            {
                Settings settings = Settings.Instance;
            }
            else
            {
                Settings.Instance = Settings.Deserialize(Settings.PATH_SETTINGS);
            }

            try
            {
                // Install latest driver.
                if (Settings.Instance.IsFirst && MessageBox.Show("최신 버전의 크롬 드라이버를 설치하시겠습니까? 이 메시지는 첫 실행시에만 나타납니다.", "Bandit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Settings.Instance.DriverVersion = new DriverUtility().GetLatestVersion();

                    DriverInstallerView installer = new DriverInstallerView(Settings.Instance.DriverVersion);
                    installer.Show();
                }
            }
            finally
            {
                Settings.Instance.IsFirst = false;
                Settings.Instance.Serialize(Settings.PATH_SETTINGS);

            }
            base.OnStartup(e);
        }

        private void OnUnhandledExceptionOccurs(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"처리되지 않은 예외가 발생했습니다.\r\n{((Exception)e.ExceptionObject).Source}\r\n{((Exception)e.ExceptionObject).Message}\r\n{((Exception)e.ExceptionObject).InnerException}\r\n{((Exception)e.ExceptionObject).StackTrace}", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);

            MessageBox.Show($"확인을 누르시면 오류가 발생한 크롬과 크롬드라이버를 강제 종료합니다. 크롬을 사용하고 계셨다면 작업을 저장해주십시오.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);

            // Kill chrome drivers.
            Process[] drivers = Process.GetProcessesByName("chromedriver.exe");

            if (drivers.Length > 0)
            {
                foreach (Process process in drivers)
                {
                    process.Kill();
                }
            }

            // Kill chromes.
            Process[] chromes = Process.GetProcessesByName("chrome.exe");

            if (chromes.Length > 0)
            {
                foreach (Process process in chromes)
                {
                    process.Kill();
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Instance.Serialize(Settings.PATH_SETTINGS);
            base.OnExit(e);
        }
    }
}
