using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ApiServerControllerBase.DTOs;

public class CreateTodoItemDto
{
    [Required]
    [MaxLength(100)]
    [Description("할 일")]
    public string Title { get; set; } = string.Empty;
}
