using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bandit.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region ::INotifyPropertyChanged Supports::

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 속성 값 변경을 이벤트 구독자에게 고지합니다.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            // take a copy to prevent thread issues.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
