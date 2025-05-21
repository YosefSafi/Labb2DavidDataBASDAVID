using Labb2DavidDataBASDAVID.Data;
using Labb2DavidDataBASDAVID.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

class Program
{
    static void Main()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BokhandelContext>();
        optionsBuilder.UseSqlServer(@"Server=GAMBINOS\SQLEXPRESS01;Database=Bokhandel;Trusted_Connection=True;TrustServerCertificate=True");

        using var context = new BokhandelContext(optionsBuilder.Options);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Bokhandel ====");
            Console.WriteLine("1. Lista lagersaldo per butik");
            Console.WriteLine("2. Lägg till bok i butik");
            Console.WriteLine("3. Ta bort bok från butik");
            Console.WriteLine("0. Avsluta");
            Console.Write("\nVal: ");
            var val = Console.ReadLine();

            switch (val)
            {
                case "1":
                    ListaLagersaldo(context);
                    break;
                case "2":
                    LaggTillBokILager(context);
                    break;
                case "3":
                    TaBortBokFranButik(context);
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Ogiltigt val.");
                    break;
            }

            Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }
    }

    static void TaBortBokFranButik(BokhandelContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Ta bort bok från butikens lager ===");

        var butiker = context.Butiker.ToList();
        Console.WriteLine("Välj butik:");
        foreach (var butik in butiker)
        {
            Console.WriteLine($"{butik.ID}. {butik.Namn}");
        }

        Console.Write("Ange butikens ID: ");
        if (!int.TryParse(Console.ReadLine(), out int butikId) || !butiker.Any(b => b.ID == butikId))
        {
            Console.WriteLine("Ogiltigt butik-ID.");
            return;
        }

        var lagersaldoForButik = context.LagerSaldo
            .Where(ls => ls.ButikID == butikId)
            .Join(context.Böcker,
                  ls => ls.ISBN,
                  bok => bok.ISBN,
                  (ls, bok) => new
                  {
                      ISBN = ls.ISBN,
                      BokTitel = bok.Titel,
                      Antal = ls.Antal
                  })
            .ToList();

        if (!lagersaldoForButik.Any())
        {
            Console.WriteLine("Denna butik har inget lagersaldo.");
            return;
        }

        Console.WriteLine("\nBöcker i lagret:");
        foreach (var bok in lagersaldoForButik)
        {
            Console.WriteLine($"{bok.ISBN} - {bok.BokTitel}, Antal: {bok.Antal}");
        }

        Console.Write("Ange ISBN för boken du vill ta bort: ");
        var isbn = Console.ReadLine();
        var lagersaldoPost = context.LagerSaldo.FirstOrDefault(ls => ls.ButikID == butikId && ls.ISBN == isbn);

        if (lagersaldoPost == null)
        {
            Console.WriteLine("Ogiltigt ISBN eller boken finns inte i lagret.");
            return;
        }

        Console.Write("Ange antal att ta bort: ");
        if (!int.TryParse(Console.ReadLine(), out int antal) || antal <= 0)
        {
            Console.WriteLine("Ogiltigt antal.");
            return;
        }

        if (antal > lagersaldoPost.Antal)
        {
            Console.WriteLine("Du försöker ta bort mer än vad som finns i lagret.");
            return;
        }

        lagersaldoPost.Antal -= antal;

        if (lagersaldoPost.Antal == 0)
        {
            context.LagerSaldo.Remove(lagersaldoPost);
        }

        context.SaveChanges();
        Console.WriteLine("\nBok borttagen från lagret!");
    }


    static void LaggTillBokILager(BokhandelContext context)
    {
        Console.Clear();
        Console.WriteLine("=== Lägg till bok i lager ===");

        var butiker = context.Butiker.ToList();
        Console.WriteLine("Välj butik:");
        foreach (var butik in butiker)
        {
            Console.WriteLine($"{butik.ID}. {butik.Namn}");
        }
        Console.Write("Ange butikens ID: ");
        if (!int.TryParse(Console.ReadLine(), out int butikId) || !butiker.Any(b => b.ID == butikId))
        {
            Console.WriteLine("Ogiltigt butik-ID.");
            return;
        }

        var böcker = context.Böcker.ToList();
        Console.WriteLine("\nVälj bok:");
        foreach (var bok in böcker)
        {
            Console.WriteLine($"{bok.ISBN} - {bok.Titel}");
        }
        Console.Write("Ange bokens ISBN: ");
        var isbn = Console.ReadLine();
        if (!böcker.Any(b => b.ISBN == isbn))
        {
            Console.WriteLine("Ogiltigt ISBN.");
            return;
        }

        Console.Write("\nAnge antal att lägga till: ");
        if (!int.TryParse(Console.ReadLine(), out int antal) || antal <= 0)
        {
            Console.WriteLine("Ogiltigt antal.");
            return;
        }

        var lagersaldo = context.LagerSaldo.FirstOrDefault(ls => ls.ButikID == butikId && ls.ISBN == isbn);
        if (lagersaldo == null)
        {
            lagersaldo = new LagerSaldo
            {
                ButikID = butikId,
                ISBN = isbn,
                Antal = antal
            };
            context.LagerSaldo.Add(lagersaldo);
        }
        else
        {
            lagersaldo.Antal += antal;
        }

        context.SaveChanges();
        Console.WriteLine("\nBok lagts till/uppdaterats i lagret!");
    }




    static void ListaLagersaldo(BokhandelContext context)
    {
        var saldo = context.LagerSaldo
            .Include(ls => ls.Bok)
            .Include(ls => ls.Butik)
            .OrderBy(ls => ls.Butik.Namn)
            .ToList();

        foreach (var post in saldo)
        {
            Console.WriteLine($"Butik: {post.Butik.Namn} - Bok: {post.Bok.Titel} - Antal: {post.Antal}");
        }
    }

    static void LäggTillBok(BokhandelContext context)
    {
        var butiker = context.Butiker.ToList();
        var böcker = context.Böcker.ToList();

        Console.WriteLine("\nTillgängliga butiker:");
        foreach (var b in butiker)
            Console.WriteLine($"{b.ID}: {b.Namn}");

        Console.Write("\nVälj butik ID: ");
        int butikID = int.Parse(Console.ReadLine());

        Console.WriteLine("\nTillgängliga böcker:");
        foreach (var bok in böcker)
            Console.WriteLine($"{bok.ISBN}: {bok.Titel}");

        Console.Write("\nAnge ISBN för boken du vill lägga till: ");
        string isbn = Console.ReadLine();

        Console.Write("Ange antal: ");
        int antal = int.Parse(Console.ReadLine());

        var befintlig = context.LagerSaldo.FirstOrDefault(l => l.ButikID == butikID && l.ISBN == isbn);
        if (befintlig != null)
        {
            befintlig.Antal += antal;
        }
        else
        {
            var nytt = new LagerSaldo
            {
                ButikID = butikID,
                ISBN = isbn,
                Antal = antal
            };
            context.LagerSaldo.Add(nytt);
        }

        context.SaveChanges();
        Console.WriteLine("Bok tillagd.");
    }

    static void TaBortBok(BokhandelContext context)
    {
        var saldo = context.LagerSaldo
            .Include(ls => ls.Bok)
            .Include(ls => ls.Butik)
            .ToList();

        foreach (var post in saldo)
        {
            Console.WriteLine($"ButikID: {post.ButikID}, ISBN: {post.ISBN} - {post.Bok.Titel}, Antal: {post.Antal}");
        }

        Console.Write("\nAnge ButikID: ");
        int butikID = int.Parse(Console.ReadLine());

        Console.Write("Ange ISBN att ta bort: ");
        string isbn = Console.ReadLine();

        var postToRemove = context.LagerSaldo.FirstOrDefault(l => l.ButikID == butikID && l.ISBN == isbn);

        if (postToRemove != null)
        {
            context.LagerSaldo.Remove(postToRemove);
            context.SaveChanges();
            Console.WriteLine("Bok borttagen från lagersaldo.");
        }
        else
        {
            Console.WriteLine("Ingen matchande post hittades.");
        }
    }
}
