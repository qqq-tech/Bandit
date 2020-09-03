using Bandit.Commands;
using Bandit.Dialogs;
using Bandit.Entities;
using Bandit.Models;
using Bandit.Utilities;
using Bandit.Views;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bandit.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        #region ::Fields::

        private BandAccount _account;

        private BandUtility _bandUtility;

        #endregion

        #region ::Constructor::

        internal LoginViewModel()
        {
            _account = BandAccount.Instance;
            _bandUtility = BandUtility.Instance;
        }

        #endregion

        #region ::Properties::

        private string _identity = string.Empty;

        public string Identity
        {
            get
            {
                return _identity;
            }
            set
            {
                _identity = value;
                RaisePropertyChanged();
            }
        }

        private string _password = string.Empty;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                RaisePropertyChanged();
            }
        }

        private string _pin = string.Empty;

        public string Pin
        {
            get
            {
                return _pin;
            }
            set
            {
                _pin = value;
                RaisePropertyChanged();
            }
        }

        private bool _isOpenDialog = false;

        public bool IsOpenDialog
        {
            get
            {
                return _isOpenDialog;
            }
            set
            {
                _isOpenDialog = value;
                RaisePropertyChanged();
            }
        }

        private object _dialog;

        public object Dialog
        {
            get
            {
                return _dialog;
            }
            set
            {
                _dialog = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ::Commands::

        private ICommand _logInCommand;

        public ICommand LogInCommand
        {
            get
            {
                return (_logInCommand) ?? (_logInCommand = new DelegateCommand(LogIn));
            }
        }

        private ICommand _pinCertificateCommand;

        public ICommand PinCertificateCommand
        {
            get
            {
                return (_pinCertificateCommand) ?? (_pinCertificateCommand = new DelegateCommand(PinCertificate));
            }
        }

        #endregion

        #region ::Methods::

        public bool IsValidEmailAddress(string input)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,10}$");
            return regex.IsMatch(input);
        }

        public bool IsValidPassword(string input)
        {
            Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])?[A-Za-z\d!@#$%^&*()_+]{8,20}$");
            return regex.IsMatch(input);
        }

        private void LogIn()
        {
            Dialog = new ProgressDialog();
            IsOpenDialog = true;

            if (!IsValidEmailAddress(Identity))
            {
                MessageBox.Show("유효하지 않은 이메일 주소입니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                IsOpenDialog = false;
                return;
            }

            if (!IsValidPassword(Password))
            {
                MessageBox.Show("비밀번호는 8자 이상, 20자 이하의 영문과 숫자, 특수기호의 나열로 구성되어야 합니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                IsOpenDialog = false;
                return;
            }

            if (!_bandUtility.IsRunning)
                _bandUtility.Start();

            var result = _bandUtility.Login(Identity, Password);

            if (result == LoginResult.IdentityFailure)
            {
                MessageBox.Show("아이디가 일치하지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                IsOpenDialog = false;
                return;
            }
            else if (result == LoginResult.PasswordFailure)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                IsOpenDialog = false;
                return;
            }
            else if (result == LoginResult.TechnicalFailure)
            {
                MessageBox.Show("기술적인 문제가 발생하였습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Error);
                IsOpenDialog = false;
                return;
            }
            else if (result == LoginResult.RequirePin)
            {
                Dialog = new PinInputDialog();
                IsOpenDialog = true;
                return;
            }
            else if (result == LoginResult.Succeed)
            {
                Dialog = new CompleteDialog();
                return;
            }
        }

        private void PinCertificate()
        {
            bool result = _bandUtility.CertifyPin(Pin);

            if (!result)
            {
                MessageBox.Show("PIN이 일치하지 않거나 알 수 없는 오류가 발생하였습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                Dialog = new CompleteDialog();
                MessageBox.Show("로그인이 완료되었습니다!", "Bandit", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        #endregion
    }
}
