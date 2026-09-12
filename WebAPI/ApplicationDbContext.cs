using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using WebAPI.Models;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Aktivnost> Aktivnosti { get; set; }
    public DbSet<Vrsta> Vrste { get; set; }
    public DbSet<Pristupacnost> Pristupacnosti { get; set; }
    public DbSet<Recenzija> Recenzije { get; set; }
    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<AktivnostPristupacnost> AktivnostPristupacnosti { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Aktivnost>().ToTable("Aktivnost");
        modelBuilder.Entity<Vrsta>().ToTable("Vrsta");
        modelBuilder.Entity<Pristupacnost>().ToTable("Pristupacnost");
        modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
        modelBuilder.Entity<Recenzija>().ToTable("Recenzija");
        modelBuilder.Entity<AktivnostPristupacnost>().ToTable("AktivnostPristupacnost");
        modelBuilder.Entity<Log>().ToTable("Log");

        modelBuilder.Entity<AktivnostPristupacnost>()
            .HasKey(ap => new { ap.AktivnostId, ap.PristupacnostId });

    }
}
