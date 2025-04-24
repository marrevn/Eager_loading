using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        // RunVersion1();  // Жадная загрузка с Include
        // RunVersion2();  // Без Include
        // RunVersion3();  // получить компании и подгрузить к ним связанных с ними пользователей через навигационное свойство Users в классе Company
        // RunVersion4();  // ThenInclude
         RunVersion5();  // Многоуровневая система данных
    }

    // Версия 1: Жадная загрузка (Include)
    static void RunVersion1()
    {
        using (var db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var microsoft = new Company { Name = "Microsoft" };
            var google = new Company { Name = "Google" };
            db.Companies.AddRange(microsoft, google);

            var tom = new User { Name = "Tom", Company = microsoft };
            var bob = new User { Name = "Bob", Company = google };
            var alice = new User { Name = "Alice", Company = microsoft };
            var kate = new User { Name = "Kate", Company = google };
            db.Users.AddRange(tom, bob, alice, kate);

            db.SaveChanges();

            var users = db.Users.Include(u => u.Company).ToList();
            foreach (var user in users)
                Console.WriteLine($"{user.Name} - {user.Company?.Name}");
        }
    }

    //  Версия 2: Без Include (данные не загружаются)
    static void RunVersion2()
    {
        using (var db = new ApplicationContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var microsoft = new Company { Name = "Microsoft" };
            var google = new Company { Name = "Google" };
            db.Companies.AddRange(microsoft, google);

            var tom = new User { Name = "Tom", Company = microsoft };
            var bob = new User { Name = "Bob", Company = google };
            var alice = new User { Name = "Alice", Company = microsoft };
            var kate = new User { Name = "Kate", Company = google };
            db.Users.AddRange(tom, bob, alice, kate);

            db.SaveChanges();

            var users = db.Users.ToList();  // Company будет null
            foreach (var user in users)
                Console.WriteLine($"{user.Name} - {user.Company?.Name}");
        }
    }

    //  Версия 3: Разные контексты 
    static void RunVersion3()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            // пересоздадим базу данных
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Company microsoft = new Company { Name = "Microsoft" };
            Company google = new Company { Name = "Google" };
            db.Companies.AddRange(microsoft, google);

            User tom = new User { Name = "Tom", Company = microsoft };
            User bob = new User { Name = "Bob", Company = google };
            User alice = new User { Name = "Alice", Company = microsoft };
            User kate = new User { Name = "Kate", Company = google };
            db.Users.AddRange(tom, bob, alice, kate);
            db.SaveChanges();
        }
        using (ApplicationContext db = new ApplicationContext())
        {
            var users = db.Users
            .Include(u => u.Company)  // добавляем данные по компаниям
            .ToList();
            foreach (var user in users)
                Console.WriteLine($"{user.Name} - {user.Company?.Name}");

            var companies = db.Companies
                                    .Include(c => c.Users)  // добавляем данные по пользователям
                                    .ToList();
            foreach (var company in companies)
            {
                Console.WriteLine(company.Name);
                // выводим сотрудников компании
                foreach (var user in company.Users)
                    Console.WriteLine(user.Name);
                Console.WriteLine("----------------------");     // для красоты
            }
        }
    }

    //  Версия 4: ThenInclude
    static void RunVersion4()
    {
        // добавление данных
        using (ApplicationContext db = new ApplicationContext())
        {
            // пересоздадим базу данных
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Country usa = new Country { Name = "USA" };
            Country japan = new Country { Name = "Japan" };
            db.Countries.AddRange(usa, japan);

            // добавляем начальные данные
            Company microsoft = new Company { Name = "Microsoft", Country = usa };
            Company sony = new Company { Name = "Sony", Country = japan };
            db.Companies.AddRange(microsoft, sony);


            User tom = new User { Name = "Tom", Company = microsoft };
            User bob = new User { Name = "Bob", Company = sony };
            User alice = new User { Name = "Alice", Company = microsoft };
            User kate = new User { Name = "Kate", Company = sony };
            db.Users.AddRange(tom, bob, alice, kate);

            db.SaveChanges();
        }
        // получение данных
        using (ApplicationContext db = new ApplicationContext())
        {
            // получаем пользователей
            var users = db.Users
                .Include(u => u.Company)  // подгружаем данные по компаниям
                    .ThenInclude(c => c!.Country)    // к компаниям подгружаем данные по странам
                .ToList();
            foreach (var user in users)
                Console.WriteLine($"{user.Name} - {user.Company?.Name} - {user.Company?.Country?.Name}");
        }
    }

    //  Версия 5: Многоуровневая система данных
    static void RunVersion5()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            // пересоздадим базу данных
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Position manager = new Position { Name = "Manager" };
            Position developer = new Position { Name = "Developer" };
            db.Positions.AddRange(manager, developer);

            City washington = new City { Name = "Washington" };
            db.Cities.Add(washington);

            Country usa = new Country { Name = "USA", Capital = washington };
            db.Countries.Add(usa);

            Company microsoft = new Company { Name = "Microsoft", Country = usa };
            Company google = new Company { Name = "Google", Country = usa };
            db.Companies.AddRange(microsoft, google);

            User tom = new User { Name = "Tom", Company = microsoft, Position = manager };
            User bob = new User { Name = "Bob", Company = google, Position = developer };
            User alice = new User { Name = "Alice", Company = microsoft, Position = developer };
            User kate = new User { Name = "Kate", Company = google, Position = manager };
            db.Users.AddRange(tom, bob, alice, kate);

            db.SaveChanges();
        }
        using (ApplicationContext db = new ApplicationContext())
        {
            // получаем пользователей
            var users = db.Users
                            .Include(u => u.Company)  // добавляем данные по компаниям
                                .ThenInclude(comp => comp!.Country)      // к компании добавляем страну 
                                    .ThenInclude(count => count!.Capital)    // к стране добавляем столицу
                            .Include(u => u.Position) // добавляем данные по должностям
                            .ToList();
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} - {user.Position?.Name}");
                Console.WriteLine($"{user.Company?.Name} - {user.Company?.Country?.Name} - {user.Company?.Country?.Capital?.Name}");
                Console.WriteLine("----------------------");     // для красоты
            }
        }
    }
}








 