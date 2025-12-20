using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankSystem.API.Models;

public class CategoryGoal
{
    [Key]
    public int Id {get; set;}
    [Required]
    public int CategoryId {get; set;}
    [Required]
    public decimal Amount {get; set;}
    [Required]
    [MaxLength(20)]
    public string Period {get; set;} = "Weekly";
    public DateOnly StartDate {get; set;}
    public DateOnly EndDate {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    // Navegation Property
    public Category Category { get; set; } = null!;

}