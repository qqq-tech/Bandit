using Bandit.Commands;
using Bandit.Dialogs;
using Bandit.Entities;
using Bandit.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Bandit.ViewModels
{
    /// <summary>
    /// 로그인 작업과 관련된 상호작용을 처리합니다.
    /// </summary>
    internal class LoginViewModel : ViewModelBase, IDisposable
    {
        #region ::Fields::

        private readonly BandUtility _bandUtility;

        #endregion

        #region ::Constructor::

        internal LoginViewModel()
        {
            _bandUtility = BandUtility.Instance;
        }

        #endregion

        #region ::Properties::

        private string _identity = string.Empty;

        /// <summary>
        /// 사용자의 ID입니다.
        /// </summary>
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

        private SecureString _password = new SecureString();

        /// <summary>
        /// 사용자의 비밀번호입니다.
        /// </summary>
        public SecureString Password
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

        /// <summary>
        /// 사용자의 인증 PIN입니다.
        /// </summary>
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

        private bool _isOpenDialog;

        /// <summary>
        /// 대화상자가 열려있는지에 대한 여부를 지정합니다.
        /// </summary>
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

        /// <summary>
        /// 사용자가 상호작용할 대화상자입니다.
        /// </summary>
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

        #region ::Validators::

        private bool IsValidEmailAddress(string input)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,10}$");
            return regex.IsMatch(input);
        }

        private bool IsValidPassword(string input)
        {
            Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])?[A-Za-z\d!@#$%^&*()_+]{8,20}$");
            return regex.IsMatch(input);
        }

        #endregion

        #region ::Login & Certify::

        private bool Validate()
        {
            // 이메일 유효성 검사.
            if (!IsValidEmailAddress(Identity))
            {
                MessageBox.Show("유효하지 않은 이메일 주소입니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            // 비밀번호 유효성 검사.
            IntPtr privateString = Marshal.SecureStringToCoTaskMemUnicode(Password);
            bool isValidPassword = IsValidPassword(Marshal.PtrToStringUni(privateString));
            Marshal.ZeroFreeCoTaskMemUnicode(privateString); // SecureString 메모리 할당 해제.

            if (!isValidPassword)
            {
                MessageBox.Show("비밀번호는 8자 이상, 20자 이하의 영문과 숫자, 특수기호의 나열로 구성되어야 합니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 로그인 작업을 수행합니다.
        /// </summary>
        private async void LogIn()
        {
            Dialog = new ProgressDialog();
            IsOpenDialog = true;

            if (!Validate())
            {
                IsOpenDialog = false;
                return;
            }

            if (!_bandUtility.IsRunning)
            {
                await _bandUtility.StartAsync();
            }

            IntPtr privateString = Marshal.SecureStringToCoTaskMemUnicode(Password);
            var result = await _bandUtility.LoginAsync(Identity, Marshal.PtrToStringUni(privateString));

            // SecureString 메모리 할당 해제.
            Marshal.ZeroFreeCoTaskMemUnicode(privateString);

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

        /// <summary>
        /// PIN 인증 작업을 수행합니다.
        /// </summary>
        private async void Certify()
        {
            Dialog = new ProgressDialog();
            bool result = await _bandUtility.CertifyAsync(Identity, Pin);

            if (!result)
            {
                Dialog = new PinInputDialog();
                MessageBox.Show("PIN이 일치하지 않거나 알 수 없는 오류가 발생하였습니다.", "Bandit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Dialog = new CompleteDialog();
                MessageBox.Show("로그인이 완료되었습니다!", "Bandit", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private ICommand _certifyCommand;

        public ICommand CertifyCommand
        {
            get
            {
                return (_certifyCommand) ?? (_certifyCommand = new DelegateCommand(Certify));
            }
        }

        #endregion

        #region ::IDisopsable Members::

        public void Dispose()
        {
            ((IDisposable)Password).Dispose();
        }

        #endregion
    }
}
