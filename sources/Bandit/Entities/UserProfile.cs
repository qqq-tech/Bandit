using System;

namespace Bandit.Entities
{
    /// <summary>
    /// 사용자의 밴드 프로필 정보를 기록하는 클래스입니다.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// 프로필 이미지의 URL 주소를 지정합니다.
        /// </summary>
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// 사용자 프로필의 이름을 지정합니다.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 새로운 사용자 프로필 인스턴스를 생성합니다.
        /// </summary>
        public UserProfile() { }

        /// <summary>
        /// 새로운 사용자 프로필 인스턴스를 생성한 후 값을 초기화합니다.
        /// </summary>
        /// <param name="image">프로필 이미지의 URL 주소를 지정합니다.</param>
        /// <param name="name">사용자 프로필의 이름을 지정합니다.</param>
        public UserProfile(Uri image, string name)
        {
            ImageUrl = image;
            Name = name;
        }
    }
}
