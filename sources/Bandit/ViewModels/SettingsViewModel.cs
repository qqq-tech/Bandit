﻿using Bandit.Commands;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Bandit.ViewModels
{
    public class SettingsViewModel :ViewModelBase
    {
        #region ::Fields:

        private readonly Settings _settings;

        private readonly DriverUtility _driver = new DriverUtility();

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

        private ObservableCollection<DateTime> _reservedTimes = new ObservableCollection<DateTime>();

        public ObservableCollection<DateTime> ReservedTimes
        {
            get
            {
                return _reservedTimes;
            }
            set
            {
                _reservedTimes = value;
                RaisePropertyChanged();
            }
        }

        private int _selectedIndex;

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

        public SettingsViewModel()
        {
            _settings = Settings.Instance;

            VersionList = new ObservableCollection<Version>(_driver.GetVersions());

            // 예약 시간 리스트 불러오기.
            foreach (DateTime time in _settings.ReservedTimes)
            {
                ReservedTimes.Add(time);
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

                CurrentVersion = SelectedVersion;
            }
            else
            {
                MessageBox.Show("같은 버전의 드라이버로는 변경할 수 없습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AddTime()
        {
            if (!ReservedTimes.Contains(SelectedTime))
            {
                ReservedTimes.Add(SelectedTime);
                ReservedTimes = new ObservableCollection<DateTime>(ReservedTimes.OrderBy(time => time));
                Settings.Instance.ReservedTimes = ReservedTimes.ToList();

                Reports.Instance.AddReport(ReportType.Added, $"'{SelectedTime.Hour}:{SelectedTime.Minute}'에 갱신 예약이 추가되었습니다.");
            }
            else
            {
                MessageBox.Show("이미 같은 시간에 갱신 예약이 되어 있습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void RemoveTime()
        {
            if (SelectedIndex != -1)
            {
                DateTime selectedTime = ReservedTimes[SelectedIndex];
                ReservedTimes.RemoveAt(SelectedIndex);
                CollectionViewSource.GetDefaultView(ReservedTimes).Refresh(); // 번호 초기화를 위해 컬렉션을 새로고침함.
                Settings.Instance.ReservedTimes = ReservedTimes.ToList();

                Reports.Instance.AddReport(ReportType.Removed, $"'{selectedTime.Hour,2:D2}:{selectedTime.Minute,2:D2}'의 갱신 예약이 제거되었습니다.");
            }
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
    }
}
