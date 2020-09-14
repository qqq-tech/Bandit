using Bandit.Utilities;
using Bandit.ViewModels;
using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Bandit.Views
{
    /// <summary>
    /// BanditView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BanditView : Window
    {
        private readonly NotifyIcon _trayIcon = new NotifyIcon();

        public BanditView()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            this.DataContext = new BanditViewModel();
        }

        private void InitializeNotifyIcon()
        {
            _trayIcon.Text = "Bandit";
            _trayIcon.Icon = Properties.Resources.bandit_icon; // 아이콘 등록.
            _trayIcon.Click += delegate // 더블 클릭 이벤트 등록.
            {
                _trayIcon.Visible = false;
                this.Show();
            };
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("프로그램을 트레이로 전환하시겠습니까?", "Bandit", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _trayIcon.Visible = true;
                this.Hide();

                e.Cancel = true; // Cancel window closing.
            }
            else if (result == MessageBoxResult.No)
            {
                if (BandUtility.Instance.IsRunning)
                {
                    BandUtility.Instance.Stop();
                }

                BandUtility.Instance.KillCurrentDriver();
                BandUtility.Instance.KillDrivers();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
