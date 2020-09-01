namespace Bandit.Entities
{
    public enum LoginResult
    {
        Succeed,
        IdentityFailure,
        PasswordFailure,
        TechnicalFailure,
        RequirePin
    }
}
