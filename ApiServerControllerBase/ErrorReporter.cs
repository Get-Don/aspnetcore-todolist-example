namespace ApiServerControllerBase;

public static class ErrorReporter
{
    /// <summary>
    /// 외부로 예외를 알린다. (예: Slack)
    /// </summary>
    public static async Task NotifyExceptionAsync(Exception? ex)
    {
        if (ex is null) return;
        Console.WriteLine(ex);

        // 여기에서 외부로 알린다.                
    }
}
