namespace BankSystem.API.Models;

public class TransactionQueryParameters: QueryParameters
{
    public decimal? MinPrice {get; set;}
    public decimal? MaxPrice {get; set;}
    public string? Store {get; set;}
}