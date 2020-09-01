namespace Bandit.Entities
{
    /// <summary>
    /// 보고서의 종류를 구분하는 열거형입니다.
    /// </summary>
    public enum ReportType
    {
        None,
        Information,
        Added,
        Changed,
        Removed,
        Complete,
        Caution,
        Warning
    }
}
