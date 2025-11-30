public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    
    // Foreign Key
    public int CompanyId { get; set; }
    
    // Navigation Properties
    public Company Company { get; set; } = default!;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
