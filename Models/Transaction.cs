using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BankSystem.API.Models;

public class Transaction
{
        [Key]
        public int Id {get; set;}
        [Required]
        public DateTime Date {get; set;}
        [MaxLength(255)]
        public string? Description {get; set;}
        [Required]
        [MaxLength(64)]
        public string Store {get; set;} = string.Empty;
        [Required]
        [Column(TypeName ="decimal(10, 2)")]
        public decimal Amount {get; set;}
        [Required]
        [MaxLength(10)]
        public TransactionType Type {get; set;}
        public bool IsSplit {get; set;} = false;
        public string? Notes {get; set;}
        public DateTime CreatedAt {get; set;}
        public DateTime UpdatedAt {get; set;}
        // Navigation Properties
        public ICollection<TransactionSplit> Splits {get; set;}
        public TransactionCategory TransactionCategory {get; set;}
}