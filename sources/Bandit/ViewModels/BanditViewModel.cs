using Bandit.Commands;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace Bandit.ViewModels
{
    public class BanditViewModel : ViewModelBase
    {
        #region ::Fields::

        private readonly Reports _reports;

        private readonly Timer _timer = new Timer();

        private readonly List<string> _postCaches = new List<string>();

        #endregion

        #region ::Constructors::

        public BanditViewModel()
        {
            // 보고서 초기화.
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
                return Account.Instance.Profile.ImageUrl;
            }
            set
            {
                Account.Instance.Profile.ImageUrl = value;
                RaisePropertyChanged();
            }
        }

        public string ProfileName
        {
            get
            {
                if (Account.Instance.Profile.Name == null)
                {
                    Account.Instance.Profile.Name = "로그인이 필요합니다";
                }

                return Account.Instance.Profile.Name;
            }
            set
            {
                Account.Instance.Profile.Name = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAccountInitialized
        {
            get
            {
                return Account.Instance.IsInitialized;
            }
            set
            {
                Account.Instance.IsInitialized = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::UI::

        private void ShowSettings()
        {
            if (!IsAccountInitialized)
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
            }
        }

        private void ShowInformation()
        {
            InformationView informationView = new InformationView();
            informationView.ShowDialog();
        }

        #endregion

        #region ::Account::

        private void AccountLogin()
        {
            if (!Account.Instance.IsInitialized)
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

        private void AccountLogout()
        {
            if (CurrentTaskState == TaskState.Loading || CurrentTaskState == TaskState.Running || CurrentTaskState == TaskState.WebProcessing)
            {
                MessageBox.Show("밴딧 작업중에는 로그아웃이 불가능합니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (Account.Instance.IsInitialized)
            {
                if (MessageBox.Show("정말 로그아웃 하시겠습니까?", "Bandit", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    return;
                }

                // 계정 정보를 초기화한다.
                Account.Instance = new Account();

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

        #endregion

        #region ::Bandit Task::

        private async void AutomaticTask(object sender, ElapsedEventArgs e)
        {
            CurrentTaskState = TaskState.WebProcessing;

            BandUtility band = BandUtility.Instance;

            List<string> posts = await band.GetFeedAsync();

            foreach (string post in posts)
            {
                if (_postCaches.Contains(post))
                {
                    continue;
                }

                await band.CheckAttendanceAsync(post);
                _postCaches.Add(post);
            }

            CurrentTaskState = TaskState.Running;
        }

        private async void ManualTask(object sender, ElapsedEventArgs e)
        {
            CurrentTaskState = TaskState.WebProcessing;

            BandUtility band = BandUtility.Instance;

            // 예약 시간 검사.
            foreach (DateTime time in Settings.Instance.ReservedTimes)
            {
                // 소수점 이하 자리를 버리기 위해 버림 함수 사용.
                if ((int)Math.Truncate(DateTime.Now.TimeOfDay.TotalMinutes) == (int)time.TimeOfDay.TotalMinutes)
                {
                    List<string> posts = await band.GetFeedAsync();

                    foreach (string post in posts)
                    {
                        if (_postCaches.Contains(post))
                            continue;

                        await band.CheckAttendanceAsync(post);
                        _postCaches.Add(post);
                    }
                }
            }

            CurrentTaskState = TaskState.Running;
        }

        private async void ControlBanditTask()
        {

            if (CurrentTaskState == TaskState.Idle)
            {
                if (!Account.Instance.IsInitialized)
                {
                    MessageBox.Show("밴딧 작업을 진행하기에 앞서 밴드 계정으로 로그인 해주시기 바랍니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (MessageBox.Show("소프트웨어 사용에 대한 면책조항\n\n" +
                    "본 소프트웨어는 Selenium 프로젝트를 이용한 웹 자동화 테스트를 위한 용도로 개발되었으며, 다른 용도로의 사용은 전혀 고려하지 않았습니다. " +
                    "또한 소프트웨어를 이용함으로써 생길 수 있는 모든 법적 문제의 책임은 사용자에게 있습니다. " +
                    "개발자는 오직 소프트웨어의 소스코드로 인해 발생한 문제에만 책임을 집니다.\n" +
                    "위 면책조항에 동의하고 작업을 시작하시려면 '예'를, 거부하시려면 '아니오'를 선택해주세요.", "Bandit", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                Reports.Instance.AddReport(ReportType.Information, "밴딧 작업을 시작합니다!");

                CurrentTaskState = TaskState.Loading;

                // 밴드 유틸리티 초기화.
                BandUtility band = BandUtility.Instance;
                _postCaches.Clear();
                _postCaches.AddRange(await band.GetFeedAsync());

                CurrentTaskState = TaskState.Running;

                if (Settings.Instance.IsAutomatic)
                {
                    _timer.Interval = Settings.Instance.RefreshInterval * 60000; // 자동 모드.
                    _timer.Elapsed += new ElapsedEventHandler(AutomaticTask);
                    _timer.Enabled = true;
                }
                else
                {
                    _timer.Interval = 60000; // 시간 지정 모드.
                    _timer.Elapsed += new ElapsedEventHandler(ManualTask);
                    _timer.Enabled = true;
                }

                _timer.Start();
            }
            else if (CurrentTaskState == TaskState.Running)
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
        }

        #endregion

        #region ::Commands::

        private ICommand _accountLoginCommand;

        public ICommand AccountLoginCommand
        {
            get
            {
                return (_accountLoginCommand) ?? (_accountLoginCommand = new DelegateCommand(AccountLogin));
            }
        }

        private ICommand _accountLogoutCommand;

        public ICommand AccountLogoutCommand
        {
            get
            {
                return (_accountLogoutCommand) ?? (_accountLogoutCommand = new DelegateCommand(AccountLogout));
            }
        }

        private ICommand _settingsCommand;

        public ICommand SettingsCommand
        {
            get
            {
                return (_settingsCommand) ?? (_settingsCommand = new DelegateCommand(ShowSettings));
            }
        }

        private ICommand _informationCommand;

        public ICommand InformationCommand
        {
            get
            {
                return (_informationCommand) ?? (_informationCommand = new DelegateCommand(ShowInformation));
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
    }
}
