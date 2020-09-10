using Bandit.Commands;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bandit.ViewModels
{
    public class DriverInstallerViewModel : ViewModelBase
    {
        #region ::Fields::

        private readonly Version _selectedVersion;

        private DriverInstallerState _state = DriverInstallerState.Start;

        private DriverUtility _driver;

        private string _tempPath;

        private int _retryCount;

        #endregion

        #region ::Properties::

        public string Title
        {
            get
            {
                return $"Chrome Driver {_selectedVersion}";
            }
        }

        private string _progressCaption = "초기화 중...";

        public string ProgressCaption
        {
            get
            {
                return _progressCaption;
            }
            set
            {
                _progressCaption = value;
                RaisePropertyChanged();
            }
        }

        private double _progressValue;

        public double ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Constructor::

        public DriverInstallerViewModel(Version version)
        {
            _selectedVersion = version;
            Reports.Instance.AddReport(ReportType.Information, "드라이버 인스톨러가 생성되었습니다.");
        }

        #endregion

        #region ::Driver Installer Methods::

        private async void Install()
        {
            if (_state == DriverInstallerState.Start)
            {
                // 드라이버 초기화.
                _driver = new DriverUtility();
                _tempPath = Path.GetTempFileName();
                Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 설치를 시작합니다.");
                _state = DriverInstallerState.Check;
                Install();
            }
            else if (_state == DriverInstallerState.Check)
            {
                await CheckAsync().ConfigureAwait(false);
            }
            else if (_state == DriverInstallerState.Download)
            {
                await DownloadAsync().ConfigureAwait(false);
            }
            else if (_state == DriverInstallerState.Decompress)
            {
                await DecompressAsync().ConfigureAwait(false);
            }
            else if (_state == DriverInstallerState.Complete)
            {
                ProgressCaption = "완료";

                Reports.Instance.AddReportWithDispatcher(ReportType.Complete, "드라이버 설치가 완료되었습니다.");
                Settings.Instance.DriverVersion = _selectedVersion;
            }
            else if (_state == DriverInstallerState.Error)
            {
                if (_retryCount >= 10)
                {
                    return;
                }

                Reports.Instance.AddReportWithDispatcher(ReportType.Complete, $"드라이버 설치중 오류가 발생하였습니다. {++_retryCount}번째 재시도 중입니다.");
                Install();
            }

        }

        private async Task CheckAsync()
        {
            var task = new Task(() =>
            {
                // 버전 검사.
                ProgressCaption = "버전 검사 중...";

                if (_driver.IsExistsVersion(_selectedVersion))
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"크롬 드라이버 {_selectedVersion}에 대한 버전 검사가 완료되었습니다.");
                    _state = DriverInstallerState.Download;
                    Install();
                }
                else
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Warning, "존재하지 않는 드라이버 버전이므로 드라이버 설치가 취소되었습니다.");
                    _state = DriverInstallerState.Error;
                    Install();
                }
            });

            task.Start();
            await task.ConfigureAwait(false);
        }

        private async Task DownloadAsync()
        {
            var task = new Task(() =>
            {
                ProgressCaption = "다운로드 중...";
                Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 다운로드를 시작합니다.");

                try
                {
                    // 드라이버 다운로드 관련 이벤트를 구독한다.
                    _driver.DownloadProgressChanged += new DriverUtility.DownloadProgressChangedEventHandler((double progress) => ProgressValue = progress);
                    _driver.DownloadDriverCompleted += new AsyncCompletedEventHandler((object sender, AsyncCompletedEventArgs e) => {
                        Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 다운로드가 완료되었습니다.");
                        _state = DriverInstallerState.Decompress;
                        Install();
                    });

                    _driver.DownloadDriverAsync(_selectedVersion, _tempPath);
                }
                catch
                {
                    _state = DriverInstallerState.Error;
                }
            });

            task.Start();
            await task.ConfigureAwait(false);
        }

        private async Task DecompressAsync()
        {
            var task = new Task(() =>
            {
                ProgressCaption = "패키지 압축 해제 중...";
                Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 패키지 압축 해제를 시작합니다.");

                try
                {
                    _driver.DecompressProgressChanged += new DriverUtility.DecompressProgressChangedEventHandler((double progress) => ProgressValue = progress);
                    _driver.DecompressFile(_tempPath, Path.Combine(Directory.GetCurrentDirectory()));
                    Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 패키지 압축 해제가 완료되었습니다.");
                    _state = DriverInstallerState.Complete;
                    Install();
                }
                catch
                {
                    _state = DriverInstallerState.Error;
                }
            });

            task.Start();
            await task.ConfigureAwait(false);
        }

        #endregion

        #region ::Commands::

        private ICommand _installCommand;

        public ICommand InstallCommand
        {
            get
            {
                return (_installCommand) ?? (_installCommand = new DelegateCommand(Install));
            }
        }

        #endregion
    }
}
