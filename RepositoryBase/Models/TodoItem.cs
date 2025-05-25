using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace RepositoryBase.Models;

[Table("t_todos")]
public class TodoItem
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column(name: "title", TypeName = "varchar(100)")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("completed_date")]
    public DateTime? CompletedAt { get; set; }

    [Column("created_date")]
    public DateTime CreatedAt { get; set; }        
}
