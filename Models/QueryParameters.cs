using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Models;

public class QueryParameters
{
    const int _maxsize = 100;
    private int _size = 50;
    public int Page {get; set;}= 1;
    public int Size
    {
        get {return _size;}
        set
        {
            _size = Math.Min(_maxsize, value);
        }
    }

    public string SortBy {get; set;} = "CreatedAt";
    private string _sortOrder = "desc";
    public string SortOrder
    {
        get
        {
            return _sortOrder;     
        }
        set
        {
            if(value == "asc" || value == "desc")
            {
                _sortOrder = value;
            }     
        }
    }

}