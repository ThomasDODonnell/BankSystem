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

}