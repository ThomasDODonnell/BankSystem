using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankSystem.API.Models;

public class TransactionSplit
{
    [Key]
    public int Id {get; set;}
    [Required]
    public int TransactionId {get; set;}
    [Required]
    public int CategoryId {get; set;}
    [Required]
    [Column(TypeName ="decimal(10, 2)")]
    public decimal Amount {get; set;}
    public string? Notes {get; set;}
    public DateTime CreatedAt {get; set;}
    // Navigation Properties
    public Transaction Transaction {get; set;} = null!;
    public Category Category {get; set;} = null!;
}
