namespace Bandit.Entities
{
    /// <summary>
    /// 로그인 작업의 결과를 알려주는 열거형입니다.
    /// </summary>
    public enum LoginResult
    {
        Succeed,
        IdentityFailure,
        PasswordFailure,
        TechnicalFailure,
        RequirePin
    }
}
