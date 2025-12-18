using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankSystem.API.Models;

public class Category
{
    [Key]
    public int Id {get; set;}
    [MaxLength(100)]
    [Required]
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    [MaxLength(7)]
    public string? Color {get; set;}
    [MaxLength(50)]
    public string? Icon {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt{get; set;}
    public CategoryGoal? CategoryGoal { get; set; }
}