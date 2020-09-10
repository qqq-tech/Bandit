using Bandit.Models;
using System;
using System.Diagnostics;
using System.Windows;

namespace Bandit
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("Bandit");
            // 같은 이름의 프로세스가 두 개 이상일 경우.
            if (procs.Length > 1)
            {
                MessageBox.Show("프로그램이 이미 실행중입니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            else
            {
                base.OnStartup(e);
                Settings.Instance = Settings.Deserialize(Settings.PATH_SETTINGS);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Instance.Serialize(Settings.PATH_SETTINGS);
        }
    }
}
