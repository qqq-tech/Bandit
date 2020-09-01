using Bandit.Entities;
using System.Collections.Generic;

namespace Bandit.Models
{
    internal class BandAccount
    {
        #region ::Singleton Supports::

        private static BandAccount _instance = null;

        /// <summary>
        /// 밴드 계정 클래스의 싱글톤 인스턴스를 불러오거나 변경합니다.
        /// </summary>
        internal static BandAccount Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BandAccount();
                    _instance.IsInitialized = false;
                    _instance.Profile = new UserProfile();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Properties::

        internal bool IsInitialized { get; set; }

        internal UserProfile Profile { get; set; }

        internal string Identity { get; set; }

        internal string Password { get; set; }

        #endregion

        #region ::Constructor::

        internal BandAccount()
        {
            IsInitialized = false;
            Profile = new UserProfile();
        }

        #endregion
    }
}
