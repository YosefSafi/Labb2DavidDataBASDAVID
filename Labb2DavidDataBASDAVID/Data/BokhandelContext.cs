using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb2DavidDataBASDAVID.Models;
using Microsoft.EntityFrameworkCore;


namespace Labb2DavidDataBASDAVID.Data
{


    public class BokhandelContext : DbContext
    {
        public BokhandelContext(DbContextOptions<BokhandelContext> options) : base(options) { }

        public DbSet<Forfattare> Forfattare { get; set; }
        public DbSet<Bok> Böcker { get; set; }
        public DbSet<Butik> Butiker { get; set; }
        public DbSet<LagerSaldo> LagerSaldo { get; set; }
        public DbSet<Kund> Kunder { get; set; }
        public DbSet<Order> Ordrar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bok>()
                .HasKey(b => b.ISBN);

            modelBuilder.Entity<LagerSaldo>()
                .HasKey(ls => new { ls.ButikID, ls.ISBN });

            modelBuilder.Entity<Order>()
                .HasKey(o => o.ID);

            modelBuilder.Entity<Bok>()
                .HasOne(b => b.Författare)
                .WithMany(f => f.Böcker)
                .HasForeignKey(b => b.FörfattarID);

            modelBuilder.Entity<LagerSaldo>()
                .HasOne(ls => ls.Butik)
                .WithMany(b => b.LagerSalden)
                .HasForeignKey(ls => ls.ButikID);

            modelBuilder.Entity<LagerSaldo>()
                .HasOne(ls => ls.Bok)
                .WithMany(b => b.LagerSalden)
                .HasForeignKey(ls => ls.ISBN);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Kund)
                .WithMany()
                .HasForeignKey(o => o.KundID);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Bok)
                .WithMany(b => b.Ordrar)
                .HasForeignKey(o => o.ISBN);
        }
    }

}