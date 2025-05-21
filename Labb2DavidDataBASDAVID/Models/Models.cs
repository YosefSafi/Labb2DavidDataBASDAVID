using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2DavidDataBASDAVID.Models
{
    public class Forfattare
    {
        public int ID { get; set; }
        public string Förnamn { get; set; }
        public string Efternamn { get; set; }
        public DateTime Födelsedatum { get; set; }

        public ICollection<Bok> Böcker { get; set; }
    }

    public class Bok
    {
        public string ISBN { get; set; }
        public string Titel { get; set; }
        public string Språk { get; set; }
        public decimal Pris { get; set; }
        public DateTime Utgivningsdatum { get; set; }

        public int FörfattarID { get; set; }
        public Forfattare Författare { get; set; }

        public ICollection<LagerSaldo> LagerSalden { get; set; }
        public ICollection<Order> Ordrar { get; set; }
    }

    public class Butik
    {
        public int ID { get; set; }
        public string Namn { get; set; }
        public string Adress { get; set; }

        public ICollection<LagerSaldo> LagerSalden { get; set; }
    }

    public class LagerSaldo
    {
        public int ButikID { get; set; }
        public Butik Butik { get; set; }

        public string ISBN { get; set; }
        public Bok Bok { get; set; }

        public int Antal { get; set; }
    }

    public class Kund
    {
        public int ID { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }

        public int KundID { get; set; }
        public Kund Kund { get; set; }

        public string ISBN { get; set; }
        public Bok Bok { get; set; }

        public int Antal { get; set; }
        public DateTime Orderdatum { get; set; }
    }

}
