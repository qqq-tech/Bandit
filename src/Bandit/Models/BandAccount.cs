using Bandit.Entities;

namespace Bandit.Models
{
    /// <summary>
    /// 밴드 계정의 정보를 기록하는 클래스 입니다.
    /// </summary>
    internal class BandAccount
    {
        #region ::Singleton Supports::

        private static BandAccount _instance;

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

        /// <summary>
        /// 계정 정보가 초기화 되었는지의 여부를 지정합니다.
        /// </summary>
        internal bool IsInitialized { get; set; }

        /// <summary>
        /// 사용자의 프로필 정보입니다.
        /// </summary>
        internal UserProfile Profile { get; set; }

        /// <summary>
        /// 새로운 밴드 계정 클래스의 인스턴스를 생성합니다.
        /// </summary>
        internal BandAccount()
        {
            IsInitialized = false;
            Profile = new UserProfile();
        }
    }
}
