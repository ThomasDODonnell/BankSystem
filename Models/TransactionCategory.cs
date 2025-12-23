using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankSystem.API.Models;

public class TransactionCategory
{
    [Key]
    public int Id {get; set;}
    [Required]
    public int TransactionId {get; set;}
    [Required]
    public int CategoryId {get; set;}
    public DateTime CreatedAt {get; set;}
    // Navigation Properties
    public Transaction Transaction {get; set;}
    public Category Category {get; set;}
}