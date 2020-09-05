using Bandit.Commands;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Bandit.ViewModels
{
    public class SettingsViewModel :ViewModelBase
    {
        #region ::Fields:

        private Settings _settings;

        private DriverUtility _utility = new DriverUtility();

        #endregion

        #region ::Properties::

        public bool IsAutomatic
        {
            get
            {
                return _settings.IsAutomatic;
            }
            set
            {
                _settings.IsAutomatic = value;
                Reports.Instance.AddReport(ReportType.Changed, $"IsAutomatic이(가) {value}(으)로 변경되었습니다.");
                RaisePropertyChanged();
            }
        }

        public int RefreshInterval
        {
            get
            {
                return _settings.RefreshInterval;
            }
            set
            {
                _settings.RefreshInterval = value;
                Reports.Instance.AddReport(ReportType.Changed, $"RefreshInterval이(가) {value}(으)로 변경되었습니다.");
                RaisePropertyChanged();
            }
        }

        public Version CurrentVersion
        {
            get
            {
                return _settings.DriverVersion;
            }
            set
            {
                _settings.DriverVersion = value;
                RaisePropertyChanged();
            }
        }

        private Version _selectedVersion = Settings.Instance.DriverVersion;

        public Version SelectedVersion
        {
            get
            {
                return _selectedVersion;
            }
            set
            {
                _selectedVersion = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Version> _versionList = new ObservableCollection<Version>();

        public ObservableCollection<Version> VersionList
        {
            get
            {
                return _versionList;
            }
            set
            {
                _versionList = value;
                RaisePropertyChanged();
            }
        }

        public int TimeOutLimit
        {
            get
            {
                return _settings.TimeOutLimit;
            }
            set
            {
                _settings.TimeOutLimit = value;
                Reports.Instance.AddReport(ReportType.Changed, $"TimeOutLimit이(가) {value}(으)로 변경되었습니다.");
                RaisePropertyChanged();
            }
        }

        public bool UseHeadless
        {
            get
            {
                return _settings.UseHeadless;
            }
            set
            {
                _settings.UseHeadless = value;
                Reports.Instance.AddReport(ReportType.Changed, $"UseHeadless이(가) {value}(으)로 변경되었습니다.");
                RaisePropertyChanged();
            }
        }

        public bool UseConsole
        {
            get
            {
                return _settings.UseConsole;
            }
            set
            {
                _settings.UseConsole = value;
                Reports.Instance.AddReport(ReportType.Changed, $"UseConsole이(가) {value}(으)로 변경되었습니다.");
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DateTime> _reservatedTimes = new ObservableCollection<DateTime>();

        public ObservableCollection<DateTime> ReservatedTimes
        {
            get
            {
                return _reservatedTimes;
            }
            set
            {
                _reservatedTimes = value;
                RaisePropertyChanged();
            }
        }

        private int _selectedIndex = 0;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _selectedTime = new DateTime(1, 1, 1, 0, 0, 0);

        public DateTime SelectedTime
        {
            get
            {
                return _selectedTime;
            }
            set
            {
                _selectedTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Constructors::

        private ObservableCollection<DateTime> ListToObservableCollection(List<DateTime> list)
        {
            ObservableCollection<DateTime> collection = new ObservableCollection<DateTime>();
            
            foreach (DateTime time in list)
            {
                collection.Add(time);
            }

            return collection;
        }

        public SettingsViewModel()
        {
            _settings = Settings.Instance;
            VersionList = new ObservableCollection<Version>(_utility.GetVersionList());
            ReservatedTimes = ListToObservableCollection(_settings.ReservatedTimes);
        }

        #endregion

        #region ::Commands::

        private ICommand _applyDriverCommand;

        public ICommand ApplyDriverCommand
        {
            get
            {
                return (_applyDriverCommand) ?? (_applyDriverCommand = new DelegateCommand(ApplyDriver));
            }
        }

        private ICommand _addTimeCommand;

        public ICommand AddTimeCommand
        {
            get
            {
                return (_addTimeCommand) ?? (_addTimeCommand = new DelegateCommand(AddTime));
            }
        }

        private ICommand _removeTimeCommand;

        public ICommand RemoveTimeCommand
        {
            get
            {
                return (_removeTimeCommand) ?? (_removeTimeCommand = new DelegateCommand(RemoveTime));
            }
        }

        #endregion

        #region ::Methods::

        private void ApplyDriver()
        {
            if (CurrentVersion != SelectedVersion)
            {
                DriverInstallerView installer = new DriverInstallerView(SelectedVersion);
                installer.ShowDialog();
                installer.Close();

                CurrentVersion = SelectedVersion;
            }
            else
            {
                MessageBox.Show("같은 버전의 드라이버로는 변경할 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AddTime()
        {
            if (!ReservatedTimes.Contains(SelectedTime))
            {
                ReservatedTimes.Add(SelectedTime);
                ReservatedTimes = new ObservableCollection<DateTime>(ReservatedTimes.OrderBy(time => time));
                Settings.Instance.ReservatedTimes = ReservatedTimes.ToList();

                Reports.Instance.AddReport(ReportType.Added, $"'{SelectedTime.Hour}:{SelectedTime.Minute}'에 갱신 예약이 추가되었습니다.");
            }
            else
            {
                MessageBox.Show("이미 같은 시간에 갱신 예약이 되어 있습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void RemoveTime()
        {
            if (SelectedIndex != -1)
            {
                DateTime selectedTime = ReservatedTimes[SelectedIndex];
                ReservatedTimes.RemoveAt(SelectedIndex);
                CollectionViewSource.GetDefaultView(ReservatedTimes).Refresh(); // 번호 초기화를 위해 컬렉션을 새로고침한다.
                Settings.Instance.ReservatedTimes = ReservatedTimes.ToList();

                Reports.Instance.AddReport(ReportType.Removed, $"'{selectedTime.Hour,2:D2}:{selectedTime.Minute,2:D2}'의 갱신 예약이 제거되었습니다.");
            }
        }

        #endregion
    }
}
