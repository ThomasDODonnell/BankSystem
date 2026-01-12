using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace BankSystem.API.Models;

public class CategoryMappings
{
    [Key]
    public int Id {get; set;}
    [Required]
    public string UserId {get; set;}
    [Required]
    public int CategoryId {get; set;}
    [Required]
    public string StoreExact {get; set;} = string.Empty;
    public string StoreContains {get; set;}
    [Required]
    public bool IsDefault {get; set;} = true;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt{get; set;}

    // Navegation Properties
    public Category Category {get; set;}
    [ForeignKey("UserId")]
    public virtual IdentityUser User {get; set;}
}