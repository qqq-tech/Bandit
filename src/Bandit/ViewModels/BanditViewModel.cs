using Bandit.Commands;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace Bandit.ViewModels
{
    public class BanditViewModel : ViewModelBase
    {
        #region ::Fields::

        private Reports _reports;

        private Timer _timer = new Timer();

        private List<string> _postCaches = new List<string>();

        #endregion

        #region ::Constructors::

        public BanditViewModel()
        {
            // Initialize settings, reports, and band account.
            _reports = Reports.Instance;
        }

        #endregion

        #region ::Properties::

        private TaskState _currentTaskState = TaskState.Idle; // 작업 상태 기본 값 : IDLE

        public TaskState CurrentTaskState
        {
            get
            {
                return _currentTaskState;
            }
            set
            {
                _currentTaskState = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Report> RecordedReports
        {
            get
            {
                return _reports.RecordedReports;
            }
            set
            {
                _reports.RecordedReports = value;
                RaisePropertyChanged();
            }
        }

        public Uri ProfileImageUrl
        {
            get
            {
                return BandAccount.Instance.Profile.ImageUrl;
            }
            set
            {
                BandAccount.Instance.Profile.ImageUrl = value;
                RaisePropertyChanged();
            }
        }

        public string ProfileName
        {
            get
            {
                if (BandAccount.Instance.Profile.Name == null)
                    BandAccount.Instance.Profile.Name = "로그인이 필요합니다";

                return BandAccount.Instance.Profile.Name;
            }
            set
            {
                BandAccount.Instance.Profile.Name = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAccountInitialized
        {
            get
            {
                return BandAccount.Instance.IsInitialized;
            }
            set
            {
                BandAccount.Instance.IsInitialized = true;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Command::

        private ICommand _accountLogInCommand;

        public ICommand AccountLogInCommand
        {
            get
            {
                return (_accountLogInCommand) ?? (_accountLogInCommand = new DelegateCommand(AccountLogIn));
            }
        }

        private ICommand _accountLogOutCommand;

        public ICommand AccountLogOutCommand
        {
            get
            {
                return (_accountLogOutCommand) ?? (_accountLogOutCommand = new DelegateCommand(AccountLogOut));
            }
        }

        private ICommand _testCommand;

        public ICommand TestCommand
        {
            get
            {
                return (_testCommand) ?? (_testCommand = new DelegateCommand(Test));
            }
        }

        private ICommand _settingsCommand;

        public ICommand SettingsCommand
        {
            get
            {
                return (_settingsCommand) ?? (_settingsCommand = new DelegateCommand(ControlSettings));
            }
        }

        private ICommand _taskCommand;

        public ICommand TaskCommand
        {
            get
            {
                return (_taskCommand) ?? (_taskCommand = new DelegateCommand(ControlBanditTask));
            }
        }

        #endregion

        #region ::Methods::

        private void AccountLogIn()
        {
            if (!BandAccount.Instance.IsInitialized)
            {
                LoginView loginView = new LoginView();
                loginView.ShowDialog();

                // View에 프로퍼티 값 변경을 알려준다.
                RaisePropertyChanged("ProfileImageUrl");
                RaisePropertyChanged("ProfileName");
                RaisePropertyChanged("IsAccountInitialized");
            }
            else
            {
                MessageBox.Show("이미 로그인이 되어 있습니다!", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void AccountLogOut()
        {
            if (CurrentTaskState == TaskState.Loading || CurrentTaskState == TaskState.Running || CurrentTaskState == TaskState.WebProcessing)
            {
                MessageBox.Show("밴딧 작업중에는 로그아웃이 불가능합니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (BandAccount.Instance.IsInitialized)
            {
                if (MessageBox.Show("정말 로그아웃 하시겠습니까?", "Bandit", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    return;

                // 계정 정보를 초기화한다.
                BandAccount.Instance = new BandAccount();

                // 밴드 유틸리티를 초기화 후 리소스를 해제한다.
                BandUtility.Instance.Stop();

                // View에 프로퍼티 값 변경을 알려준다.
                RaisePropertyChanged("ProfileImageUrl");
                RaisePropertyChanged("ProfileName");
                RaisePropertyChanged("IsAccountInitialized");

                CurrentTaskState = TaskState.Idle;

                MessageBox.Show("로그아웃 되었습니다!", "Bandit", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("로그인이 되어 있지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Test()
        {
            if (!BandAccount.Instance.IsInitialized)
            {
                TestView testView = new TestView();
                testView.ShowDialog();
            }
            else
            {
                MessageBox.Show("로그아웃을 하셔야만 개발자 메뉴로 진입할 수 있습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void ControlSettings()
        {
            if (!BandAccount.Instance.IsInitialized)
            {
                SettingsView settingsView = new SettingsView();
                settingsView.ShowDialog();

                // View에 프로퍼티 값 변경을 알려준다.
                RaisePropertyChanged("ProfileImageUrl");
                RaisePropertyChanged("ProfileName");
                RaisePropertyChanged("IsAccountInitialized");
            }
            else
            {
                MessageBox.Show("로그아웃을 하셔야만 설정을 변경할 수 있습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void ControlBanditTask()
        {
            if (CurrentTaskState == TaskState.Running)
            {
                _timer.Stop();
                CurrentTaskState = TaskState.Stopping;
                return;
            }
            else if (CurrentTaskState == TaskState.Stopping)
            {
                _timer.Start();
                CurrentTaskState = TaskState.Running;
                return;
            }
            else if (CurrentTaskState == TaskState.Loading || CurrentTaskState == TaskState.WebProcessing)
            {
                MessageBox.Show("밴딧 작업 진행중에는 작업 상태를 변경할 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else if (CurrentTaskState == TaskState.Idle)
            {
                if (!BandAccount.Instance.IsInitialized)
                {
                    MessageBox.Show("밴딧 작업을 진행하기에 앞서 밴드 계정으로 로그인 해주시기 바랍니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (MessageBox.Show("소프트웨어 사용에 대한 면책조항\n\n" +
                    "본 소프트웨어(이하 '소프트웨어')는 Selenium 프로젝트를 이용한 웹 자동화 테스트를 위한 용도로 개발되었으며, 다른 용도로의 사용은 전혀 고려하지 않았습니다. " +
                    "또한 소프트웨어를 이용함으로써 생길 수 있는 모든 법적 문제의 책임은 사용자에게 있습니다. " +
                    "개발자는 오직 소프트웨어의 소스코드로 인해 발생한 문제에만 책임을 가집니다.", "Bandit", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                Reports.Instance.AddReport(ReportType.Information, "밴딧 작업을 시작합니다!");

                CurrentTaskState = TaskState.Loading;

                BandUtility band = BandUtility.Instance;
                _postCaches.Clear();
                _postCaches.AddRange(band.GetFeed());

                CurrentTaskState = TaskState.Running;

                if (Settings.Instance.IsAutomatic)
                {
                    _timer.Interval = Settings.Instance.RefreshInterval * 60000; // 자동 모드.
                    _timer.Enabled = true;

                    _timer.Elapsed += new ElapsedEventHandler((sender, e) =>
                    {
                        CurrentTaskState = TaskState.WebProcessing;

                        List<string> posts = band.GetFeed();

                        foreach (string post in posts)
                        {
                            if (_postCaches.Contains(post))
                                continue;

                            band.CheckAttendance(post);
                            _postCaches.Add(post);
                        }

                        CurrentTaskState = TaskState.Running;
                    });
                }
                else
                {
                    _timer.Interval = 60000; // 시간 지정 모드.
                    _timer.Enabled = true;

                    _timer.Elapsed += new ElapsedEventHandler((sender, e) =>
                    {
                        CurrentTaskState = TaskState.WebProcessing;

                        foreach (DateTime time in Settings.Instance.ReservatedTimes)
                        {
                            if (DateTime.Now.TimeOfDay.TotalMinutes == time.TimeOfDay.TotalMinutes)
                            {
                                List<string> posts = band.GetFeed();

                                foreach (string post in posts)
                                {
                                    if (_postCaches.Contains(post))
                                        continue;

                                    band.CheckAttendance(post);
                                    _postCaches.Add(post);
                                }
                            }
                        }

                        CurrentTaskState = TaskState.Running;
                    });
                }

                _timer.Start();
            }
        }

        #endregion
    }
}
