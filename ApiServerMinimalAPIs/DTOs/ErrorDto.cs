using System.ComponentModel;

namespace ApiServerMinimalAPIs.DTOs;

public class ErrorDto
{
    [Description("오류 메시지")]
    public string Message { get; set; } = string.Empty;
}
