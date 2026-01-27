namespace BankSystem.API.Models;

public class TransactionQueryParameters: QueryParameters
{
    public decimal? MinPrice {get; set;}
    public decimal? MaxPrice {get; set;}
    public string? Store {get; set;}
}

public class TransactionUpdate
{
    public DateTime Date {get; set;}    
    public string Description {get; set;}
    public string Store {get; set;}
    public bool IsRecurring {get; set;}
    public decimal Amount {get; set;}
    public TransactionType Type {get; set;}
    public bool IsSplit {get; set;}
    public string Notes {get; set;}
}