using Bandit.Entities;
using Bandit.Models;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Bandit.Utilities
{
    /// <summary>
    /// 네이버 밴드와 관련된 기능을 제공하는 클래스입니다.
    /// </summary>
    public class BandUtility
    {
        #region ::Singleton Supports::

        private static BandUtility _instance;

        /// <summary>
        /// 보고서 클래스의 싱글톤 인스턴스를 불러오거나 변경합니다.
        /// </summary>
        public static BandUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BandUtility();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Fields::

        private ChromeDriverService _driverService;
        private ChromeDriver _driver;
        private WebDriverWait _wait;

        #endregion

        #region ::Properties::

        public bool IsRunning
        {
            get
            {
                if (_driverService == null)
                {
                    return false;
                }

                return _driverService.IsRunning;
            }
        }

        public int ProcessId
        {
            get
            {
                if (_driverService == null)
                {
                    return -1;
                }

                return _driverService.ProcessId;
            }
        }

        #endregion

        #region ::General::

        public void KillCurrentDriver()
        {
            if (IsRunning)
            {
                // 크롬 드라이버 프로세스를 종료한다.
                Process.GetProcessById(_driverService.ProcessId).Kill();
            }
        }

        public void KillDrivers()
        {
            Process[] processList = Process.GetProcessesByName("chromedriver.exe");

            foreach (Process processList_item in processList)
            {
                processList_item.Kill();
            }
        }

        public async Task StartAsync()
        {
            var task = new Task(() =>
            {
                if (!IsRunning)
                {
                    try
                    {
                        KillDrivers();

                        // 드라이버 서비스를 초기화한다.
                        string driverPath = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver.exe");
                        Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"크롬 드라이버 경로가 '{driverPath}'(으)로 지정되었습니다.");
                        _driverService = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory(), driverPath);

                        if (!Settings.Instance.UseConsole)
                        {
                            _driverService.HideCommandPromptWindow = true; // 콘솔 사용 여부를 지정한다.
                        }

                        // 크롬 설정을 초기화한다.
                        ChromeOptions options = new ChromeOptions();
                        options.AddArguments($"user-data-dir={Directory.GetCurrentDirectory()}/data/profile"); // 사용자 데이터(쿠키)가 저장될 디렉토리를 지정한다.
                        options.AddArguments("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.106 Safari/537.36");

                        options.AddUserProfilePreference($"profile", new
                        {
                            default_content_setting_values = new
                            {
                                //cookies = 2,
                                images = 2,
                                plugins = 2,
                                popups = 2,
                                geolocation = 2,
                                notifications = 2,
                                auto_select_certificate = 2,
                                fullscreen = 2,
                                mouselock = 2,
                                mixed_script = 2,
                                media_stream = 2,
                                media_stream_mic = 2,
                                media_stream_camera = 2,
                                protocol_handlers = 2,
                                ppapi_broker = 2,
                                automatic_downloads = 2,
                                midi_sysex = 2,
                                push_messaging = 2,
                                ssl_cert_decisions = 2,
                                metro_switch_to_desktop = 2,
                                protected_media_identifier = 2,
                                app_banner = 2,
                                site_engagement = 2,
                                durable_storage = 2
                            }
                        });

                        if (Settings.Instance.UseHeadless)
                        {
                            options.AddArgument("headless");
                            options.AddArgument("window-size=1920x1080");
                            options.AddArgument("disable-gpu");
                        }

                        // 크롬 드라이버를 초기화한다.
                        _driver = new ChromeDriver(_driverService, options);
                        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Settings.Instance.TimeOutLimit); // 암시적 타임아웃 대기 시간 지정.
                        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(Settings.Instance.TimeOutLimit); // 페이지 타임아웃 대기 시간 지정.

                        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(Settings.Instance.TimeOutLimit)); // 공용 웹 드라이버 대기 시간 지정.
                        Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"크롬 드라이버가 실행되었습니다.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"사용자의 컴퓨터에 설치된 크롬과 크롬 드라이버가 호환되지 않습니다. {ex.StackTrace}");
                        MessageBox.Show($"사용자의 컴퓨터에 설치된 크롬과 크롬 드라이버가 호환되지 않습니다. {ex.Message}\r\n{ex.StackTrace}", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                        KillCurrentDriver();
                    }
                }
                else
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"크롬 드라이버 실행이 거부되었습니다.");
                    MessageBox.Show("크롬 드라이버가 이미 실행중입니다!", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            task.Start();
            await task.ConfigureAwait(false);
        }

        public void Stop()
        {
            if (_driver != null)
            {
                _driver.Quit();
                KillCurrentDriver();
                KillDrivers();
                Reports.Instance.AddReport(ReportType.Information, $"크롬 드라이버가 중지되었습니다.");
            }
            else
            {
                KillCurrentDriver();
                KillDrivers();
                Environment.Exit(0);
                Reports.Instance.AddReport(ReportType.Information, $"크롬 드라이버 중지가 거부되었습니다.");
                MessageBox.Show("크롬 드라이버가 실행중이지 않아 중지가 불가능합니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region ::Login::

        public async Task<LoginResult> LoginAsync(string identity, string password)
        {
            var task = new Task<LoginResult>(() =>
            {
                if (!IsRunning)
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"로그인 작업이 거부되었습니다.");
                    MessageBox.Show("크롬 드라이버가 동작하고 있지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                    return LoginResult.TechnicalFailure;
                }

                // 로그인 시퀀스를 시작한다.
                _driver.Navigate().GoToUrl(Settings.URL_EMAIL_LOGIN_ID);

                // 아이디 입력을 위한 대리자를 선언 및 초기화한다.
                Func<IWebDriver, bool> identityTask = (web) =>
                {
                    try
                    {
                        IWebElement element = web.FindElement(By.Id("input_email"));
                        element.SendKeys(identity);

                        element = web.FindElement(By.XPath("//*[@id='email_login_form']/button"));
                        element.Click();

                        return true;
                    }
                    catch (NoSuchElementException ex)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Warning, $"ID 입력 필드를 찾을 수 없습니다. {ex.StackTrace}");
                        return false;
                    }
                };

                // 아이디 입력 완료 대기.
                if (!_wait.Until(identityTask))
                {
                    return LoginResult.IdentityFailure;
                }

                // 페이지 전환 여부를 통해 ID 일치 여부를 확인한다.
                if (_driver.Url.Contains(Settings.URL_EMAIL_LOGIN_ID))
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"ID가 일치하지 않습니다.");
                    return LoginResult.IdentityFailure;
                }

                // 비밀번호 입력을 위한 대리자를 선언 및 초기화한다.
                Func<IWebDriver, bool> passwordTask = (web) =>
                {
                    try
                    {
                        IWebElement element = web.FindElement(By.Id("pw"));
                        element.SendKeys(password);

                        element = web.FindElement(By.XPath("//*[@id='email_password_login_form']/button"));
                        element.Click();

                        return true;
                    }
                    catch (NoSuchElementException ex)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Warning, $"ID 입력 필드를 찾을 수 없습니다. {ex.StackTrace}");
                        return false;
                    }
                };

                // 비밀번호 입력 완료 대기.
                if (!_wait.Until(passwordTask))
                {
                    return LoginResult.PasswordFailure;
                }

                // 페이지 전환 여부를 통해 PW 일치 여부를 확인한다.
                if (_driver.Url.Contains(Settings.URL_EMAIL_LOGIN_PW))
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"비밀번호가 일치하지 않습니다.");
                    return LoginResult.PasswordFailure;
                }

                // 로그인이 완료되었는지, PIN 인증이 필요한 지의 여부를 확인한다.
                if (_driver.Url.Contains(Settings.URL_EMAIL_LOGIN_PIN))
                {
                    return LoginResult.RequirePin;
                }
                else
                {
                    Account.Instance.Profile.Name = identity;
                    Account.Instance.IsInitialized = true;

                    return LoginResult.Succeed;
                }
            });
            task.Start();
            await task.ConfigureAwait(false);

            return task.Result;
        }

        public async Task<bool> CertifyAsync(string identity, string pin)
        {
            var task = new Task<bool>(() =>
            {
                if (!IsRunning)
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"로그인 작업이 거부되었습니다.");
                    MessageBox.Show("크롬 드라이버가 동작하고 있지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                bool result = false;

                // PIN 입력을 위한 대리자를 선언 및 초기화한다.
                Func<IWebDriver, bool> pinTask = (web) =>
                {
                    try
                    {
                        IWebElement element = web.FindElement(By.Id("code"));
                        element.Clear();
                        element.SendKeys(pin);

                        element = web.FindElement(By.Id("trust"));
                        element.Click();

                        element = web.FindElement(By.XPath("//*[@id='inputForm']/button[1]"));
                        element.Click();

                        return true;
                    }
                    catch (NoSuchElementException ex)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Warning, $"PIN 입력 필드를 찾을 수 없습니다. {ex.StackTrace}");
                        return false;
                    }
                };

                if (!_wait.Until(pinTask))
                {
                    return false;
                }

                Func<IWebDriver, bool> pinValidTask = (web) =>
                {
                    try
                    {
                        IWebElement element = web.FindElement(By.Id("errorMessage"));
                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                };

                // PIN 인증 여부 확인.
                try
                {
                    if (_wait.Until(pinValidTask))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    result = false;
                }

                // 결괏값 반환.
                if (!result)
                {
                    Account.Instance = new Account(); // 계정 정보 초기화.
                    return false;
                }
                else
                {
                    Account.Instance.Profile.Name = identity;
                    Account.Instance.IsInitialized = true;

                    return true;
                }
            });
            task.Start();
            await task.ConfigureAwait(false);

            return task.Result;
        }

        #endregion

        #region ::Feed::

        public async Task<List<string>> GetFeedAsync()
        {
            var task = new Task<List<string>>(() =>
            {
                if (Account.Instance.IsInitialized)
                {
                    if (!IsRunning)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"새 글 피드 가져오기 작업이 거부되었습니다.");
                        MessageBox.Show("크롬 드라이버가 동작하고 있지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }

                    if (_driver.Url == Settings.URL_FEEDS_PAGE)
                    {
                        _driver.Navigate().Refresh();
                    }
                    else
                    {
                        _driver.Navigate().GoToUrl(Settings.URL_FEEDS_PAGE);
                    }

                    Func<IWebDriver, HtmlDocument> postTask = (web) =>
                    {
                        try
                        {
                            new WebDriverWait(_driver, TimeSpan.FromSeconds(Settings.Instance.TimeOutLimit)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[@class='_feedListRegion']")));

                            HtmlDocument document = new HtmlDocument();
                            document.LoadHtml(web.PageSource);
                            return document;
                        }
                        catch (NoSuchElementException ex)
                        {
                            Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"새 글 피드를 가져올 수 없습니다. {ex.StackTrace}");
                            MessageBox.Show("새 글 피드 소스를 가져올 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                    };

                    // 로딩 대기.
                    HtmlDocument document = _wait.Until(postTask);

                    if (document != null)
                    {
                        // 피드 파싱.
                        HtmlNode postsRoot = document.DocumentNode.SelectSingleNode("//div[@class='_feedListRegion']").SelectSingleNode("//div[@data-viewname='DFeedListView']");
                        HtmlNodeCollection posts = postsRoot.SelectNodes("//section[@data-viewname='DFeedItemView']");

                        List<string> postList = new List<string>();

                        // 주소 파싱.
                        foreach (HtmlNode post in posts)
                        {
                            postList.Add(post.SelectSingleNode("//a[@class='time']").Attributes["href"].Value);
                        }

                        Reports.Instance.AddReportWithDispatcher(ReportType.Complete, $"새 글 피드를 갱신했습니다.");
                        return postList;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            });
            task.Start();
            await task.ConfigureAwait(false);

            return task.Result;
        }

        #endregion

        #region ::Attendance::

        public async Task CheckAttendanceAsync(string url)
        {
            var task = new Task(() =>
            {
                if (Account.Instance.IsInitialized)
                {
                    if (!IsRunning)
                    {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"출석 작업이 거부되었습니다.");
                        MessageBox.Show("크롬 드라이버가 동작하고 있지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _driver.Navigate().GoToUrl(url);

                    Func<IWebDriver, HtmlDocument> postTask = (web) =>
                    {
                        try
                        {
                            new WebDriverWait(_driver, TimeSpan.FromSeconds(Settings.Instance.TimeOutLimit)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div[@class='postMain']")));

                            HtmlDocument document = new HtmlDocument();
                            document.LoadHtml(web.PageSource);
                            return document;
                        }
                        catch (NoSuchElementException ex)
                        {
                            Reports.Instance.AddReportWithDispatcher(ReportType.Caution, $"게시글 소스를 가져올 수 없습니다. {ex.StackTrace}");
                            MessageBox.Show("게시글 소스를 가져올 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                    };

                    // 로딩 대기.
                    HtmlDocument document = _wait.Until(postTask);

                    if (document == null)
                    {
                        return;
                    }

                    // 출석 목록 파싱.
                    HtmlNode[] attendanceNodes = document.DocumentNode.SelectSingleNode("//div[@class='postMain']").SelectNodes("//div[@data-viewname='DPostAttendanceCheckView']").ToArray() ?? new HtmlNode[] {};
                    List<string> attendeeIdList = new List<string>();

                    for (int index = 0; index < attendanceNodes.Length; index++)
                    {
                        string id = attendanceNodes[index].SelectSingleNode("//label[@class='etc']").Attributes["for"].Value;
                        attendeeIdList.Add(id);
                    }

                    // 출석 체크를 위한 대리자를 선언 및 초기화한다.
                    Func<IWebDriver, bool> attendanceTask = (web) =>
                    {
                        try
                        {
                            foreach (string attendeeId in attendeeIdList)
                            {
                                IWebElement element = web.FindElement(By.Id(attendeeId));
                                element.Click();
                            }

                            return true;
                        }
                        catch (NoSuchElementException ex)
                        {
                            Reports.Instance.AddReportWithDispatcher(ReportType.Warning, $"ID 입력 필드를 찾을 수 없습니다. {ex.StackTrace}");
                            return false;
                        }
                    };

                    // 출석 체크 완료 대기.
                    if (!_wait.Until(attendanceTask))
                    {
                        return;
                    }

                    string postTitle = document.DocumentNode.SelectSingleNode("//div[@class='item -attendance']").SelectSingleNode("//p[@class='addTitle']").InnerText;
                    Reports.Instance.AddReportWithDispatcher(ReportType.Complete, $"'{postTitle}' 출석 처리를 완료했습니다.");
                }
            });
            task.Start();
            await task.ConfigureAwait(false);
        }

        #endregion
    }
}
