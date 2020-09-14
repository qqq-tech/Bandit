namespace Bandit.Entities
{
    /// <summary>
    /// 애플리케이션의 작업 상태를 나타내는 열거형입니다.
    /// </summary>
    public enum TaskState
    {
        Idle,
        Running,
        Loading,
        WebProcessing,
        Stopping
    }
}
