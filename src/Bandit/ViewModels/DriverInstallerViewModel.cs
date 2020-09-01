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

        private Settings _settings;

        private Version _selectedVersion;

        private DriverUtility _driver;

        private string _tempPath;

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

        private double _progressValue = 0.0f;

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
            _settings = Settings.Instance;
            _selectedVersion = version;

            Reports.Instance.AddReport(ReportType.Information, "드라이버 인스톨러가 생성되었습니다.");
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

        #region ::Methods::

        private void Install()
        {
            Task.Run(() =>
            {
                Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 설치를 시작합니다.");

                _driver = new DriverUtility();
                _tempPath = Path.GetTempFileName();

                ProgressCaption = "버전 검사 중...";
                if (_driver.IsExistsVersion(_selectedVersion))
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Information, $"Chrome Driver {_selectedVersion}에 대한 버전 검사가 완료되었습니다.");

                    Download(); // 다운로드 시퀀스로 이행한다.
                }
                else
                {
                    Reports.Instance.AddReportWithDispatcher(ReportType.Warning, "존재하지 않는 드라이버 버전이므로 드라이버 설치가 취소되었습니다.");
                    return;
                }
            });
        }

        private void Download()
        {
            ProgressCaption = "다운로드 중...";

            // 드라이버 다운로드 관련 이벤트를 구독한다.
            _driver.DownloadProgressChanged += new DriverUtility.DownloadProgressChangedEventHandler((double progress) => ProgressValue = progress);
            _driver.DownloadDriverCompleted += new AsyncCompletedEventHandler((object sender, AsyncCompletedEventArgs e) => {
                Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 다운로드가 완료되었습니다.");
                Decompress(); // 압축 해제 시퀀스로 이행한다.
            });

            Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 다운로드를 시작합니다.");
            _driver.DownloadDriverAsync(_selectedVersion, _tempPath);
        }

        private void Decompress()
        {
            ProgressCaption = "패키지 압축 해제 중...";

            Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 패키지 압축 해제를 시작합니다.");
            _driver.DecompressProgressChanged += new DriverUtility.DecompressProgressChangedEventHandler((double progress) => ProgressValue = progress);
            _driver.DecompressFile(_tempPath, Path.Combine(Directory.GetCurrentDirectory()));
            Reports.Instance.AddReportWithDispatcher(ReportType.Information, "드라이버 패키지 압축 해제가 완료되었습니다.");

            ProgressCaption = "완료";

            Reports.Instance.AddReportWithDispatcher(ReportType.Complete, "드라이버 설치가 완료되었습니다.");
            _settings.DriverVersion = _selectedVersion;
        }

        #endregion
    }
}
