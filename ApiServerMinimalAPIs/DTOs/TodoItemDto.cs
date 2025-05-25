using System.ComponentModel;

namespace ApiServerMinimalAPIs.DTOs;

public class TodoItemDto
{
    [Description("고유 ID")]
    public int Id { get; set; }

    [Description("할 일")]
    public string Title { get; set; } = string.Empty;

    [Description("완료 유무")]
    public bool IsCompleted { get; set; }

    [Description("완료일")]
    public DateTime CreatedAt { get; set; }

    [Description("생성일")]
    public DateTime? CompletedAt { get; set; }
}
