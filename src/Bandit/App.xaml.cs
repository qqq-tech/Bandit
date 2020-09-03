using Bandit.Models;
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
            base.OnStartup(e);

            Settings.Instance.Deserialize(Settings.PATH_SETTINGS);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Instance.Serialize(Settings.PATH_SETTINGS);
        }
    }
}
