namespace BankSystem.API.Models;

public class CategoryQueryParameters: QueryParameters
{
    public string? Name {get; set;}
}

public class CategoryWithGoalRequest
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public decimal GoalAmount { get; set; }
    public string GoalPeriod { get; set; } // e.g., "monthly"
}

public class CategoryWithGoalResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal GoalAmount { get; set; }
    public string GoalPeriod { get; set; }
}

public class CategoryUpdate
{
    public string Name {get; set;}
    public string Description {get; set;}
    public string Color {get; set;}
    public string Icon {get; set;}
}