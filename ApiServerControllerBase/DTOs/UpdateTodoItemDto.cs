using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApiServerControllerBase.DTOs;

public class UpdateTodoItemDto
{
    [Description("할 일")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Description("완료 유무")]
    public bool IsCompleted { get; set; }
}
