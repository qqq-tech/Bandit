using Bandit.Models;
using Bandit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bandit.Views
{
    /// <summary>
    /// TestView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestView : Window
    {
        #region ::Fields::

        private BandUtility _bandUtility;

        #endregion

        public TestView()
        {
            InitializeComponent();
            _bandUtility = new BandUtility();
        }

        private void OnManualTaskStartClick(object sender, RoutedEventArgs e)
        {
            if (!_bandUtility.IsRunning)
                _bandUtility.Start();

            _bandUtility.ManualTaskStart();
        }

        private void OnLoadProfilesClick(object sender, RoutedEventArgs e)
        {
            if (!_bandUtility.IsRunning)
                _bandUtility.Start();

            BandAccount.Instance.IsInitialized = true;
            _bandUtility.LoadProfileImage();
        }
    }
}
