using Bandit.Utilities;
using Bandit.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Bandit.Models
{
    /// <summary>
    /// 애플리케이션 사용에 필요한 공통 설정을 기록하는 클래스입니다.
    /// </summary>
    public class Settings
    {
        #region ::Singleton Supports::

        private static Settings _instance;

        /// <summary>
        /// 설정 클래스의 싱글톤 인스턴스를 불러오거나 변경합니다.
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Consts::

        /// <summary>
        /// 설정 파일의 경로입니다.
        /// </summary>
        public static readonly string PATH_SETTINGS = @".\data\settings.json";

        /// <summary>
        /// 버전 파일의 경로입니다.
        /// </summary>
        public static readonly string PATH_VERSION = @".\data\version.dat";

        /// <summary>
        /// 크롬 드라이버 실행 파일의 경로입니다.
        /// </summary>
        public static readonly string PATH_CHROMEDRIVER = @".\chromedriver.exe";

        /// <summary>
        /// 밴딧 버전 히스토리 파일의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_BANDIT_HISTORY = "https://raw.githubusercontent.com/junimiso04/Bandit/master/contents/HISTORY.md";

        /// <summary>
        /// 밴딧 최신 버전 히스토리 파일의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_BANDIT_HISTORY_LATEST = "https://raw.githubusercontent.com/junimiso04/Bandit/master/contents/HISTORY_LATEST";

        /// <summary>
        /// 밴딧의 최신 릴리스 버전을 가져올 수 있는 URL 주소입니다.
        /// </summary>
        public static readonly string URL_BANDIT_LATEST_VERSION = "https://raw.githubusercontent.com/junimiso04/Bandit/master/contents/LATEST";

        /// <summary>
        /// 밴딧 릴리스 페이지의 형식입니다. string.Format() 메소드를 이용하여 버전 정보를 입력한 후 사용하십시오.
        /// </summary>
        public static readonly string URL_BANDIT_RELEASE_LATEST = "https://github.com/junimiso04/Bandit/releases/latest";

        /// <summary>
        /// 크롬 드라이버의 다운로드 가능한 버전 리스트의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_CHROMEDRIVERS_DOWNLOAD_LIST = "http://chromedriver.storage.googleapis.com/";

        /// <summary>
        /// 크롬 드라이버의 최신 릴리스 버전을 가져올 수 있는 URL 주소입니다.
        /// </summary>
        public static readonly string URL_CHROMEDRIVERS_LATEST = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE";

        /// <summary>
        /// 크롬 드라이버를 다운로드 할 수 있는 URL 주소입니다. string.Format() 메소드를 이용하여 버전 정보를 입력한 후 사용하십시오.
        /// </summary>
        public static readonly string URL_CHROMEDRIVERS_DOWNLOAD = "https://chromedriver.storage.googleapis.com/{0}/chromedriver_win32.zip";

        /// <summary>
        /// 밴드 이메일 로그인 아이디 입력 페이지의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_EMAIL_LOGIN_ID = "https://auth.band.us/email_login";

        /// <summary>
        /// 밴드 이메일 로그인 비밀번호 입력 페이지의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_EMAIL_LOGIN_PW = "https://auth.band.us/continue_email_login";

        /// <summary>
        /// 밴드 이메일 로그인 PIN 입력 페이지의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_EMAIL_LOGIN_PIN = "https://auth.band.us/b/validation/phone_number";

        /// <summary>
        /// 밴드 메인 페이지의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_BAND = "https://band.us";

        /// <summary>
        /// 밴드 새 글 피드 페이지의 URL 주소입니다.
        /// </summary>
        public static readonly string URL_FEEDS_PAGE = "https://band.us/feed";

        #endregion

        #region ::Properties::

        /// <summary>
        /// 프로그램이 처음 실행되었는지를 판단합니다.
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 사용자가 현재 사용중인 크롬 드라이버의 버전입니다.
        /// </summary>
        public Version DriverVersion { get; set; }

        /// <summary>
        /// 일정 간격마다 자동으로 새 글 피드를 색인할 지에 대한 여부를 지정합니다.
        /// </summary>
        public bool IsAutomatic { get; set; }

        /// <summary>
        /// 새 글 피드의 데이터를 새로고침할 시간 간격을 지정합니다. 시간 단위는 minute(일반 분)을(를) 사용합니다.
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// 웹 작업중 작업 시간이 오래 걸릴 경우 대기하는 시간을 설정합니다. 시간 단위는 second(일반 초)을(를) 사용합니다.
        /// </summary>
        public int TimeOutLimit { get; set; }

        /// <summary>
        /// 웹 작업 시 창을 띄울지에 대한 여부를 지정합니다.
        /// </summary>
        public bool UseHeadless { get; set; }

        /// <summary>
        /// 웹 작업 시 콘솔 창을 띄울지에 대한 여부를 지정합니다.
        /// </summary>
        public bool UseConsole { get; set; }

        /// <summary>
        /// 예약된 시간 목록을 저장하는 목록입니다.
        /// </summary>
        public List<DateTime> ReservedTimes { get; set; }

        #endregion

        #region ::Constructors::

        /// <summary>
        /// 새로운 설정 클래스의 인스턴스를 생성합니다.
        /// </summary>
        public Settings()
        {
            IsFirst = true;
            DriverVersion = new Version(85, 0, 4183, 8300);
            IsAutomatic = true;
            RefreshInterval = 5;
            UseHeadless = true;
            UseConsole = false;
            ReservedTimes = new List<DateTime>();
            TimeOutLimit = 10;
        }

        #endregion

        #region ::Json Formatter::

        /// <summary>
        /// 현재 인스턴스를 Json 형식으로 Serialize(직렬화)합니다.
        /// </summary>
        /// <param name="filePath">직렬화 된 파일이 저장될 경로입니다.</param>
        public void Serialize(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string jsonString = JsonConvert.SerializeObject(this);
            FileUtility.WriteTextFile(filePath, jsonString, Encoding.UTF8);
        }

        /// <summary>
        /// 지정된 Json 파일을 Settings 클래스로 Deserialize(역직렬화)합니다.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Settings Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Settings();
            }

            string jsonString = FileUtility.ReadTextFile(filePath, Encoding.UTF8);
            Settings settings = JsonConvert.DeserializeObject<Settings>(jsonString);

            if (Instance.ReservedTimes == null)
            {
                Instance.ReservedTimes = new List<DateTime>();
            }

            return settings;
        }

        #endregion
    }
}
