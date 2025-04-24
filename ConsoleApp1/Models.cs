using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=helloapp.db");
    }
}
// столица страны
public class City
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
// страна компании
public class Country
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int CapitalId { get; set; }
    public City? Capital { get; set; }  // столица страны
    public List<Company> Companies { get; set; } = new();
}
public class Company
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public List<User> Users { get; set; } = new();
}
// должность пользователя
public class Position
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<User> Users { get; set; } = new();
}
public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public int? PositionId { get; set; }
    public Position? Position { get; set; }
}