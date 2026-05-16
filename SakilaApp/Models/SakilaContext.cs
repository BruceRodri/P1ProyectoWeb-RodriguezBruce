using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Models;
using System.Reflection.Emit;

namespace SakilaApp.Models;

public class SakilaContext : IdentityDbContext
{
    public SakilaContext(DbContextOptions<SakilaContext> options) : base(options) { }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Film> Films { get; set; }
    public DbSet<FilmActor> FilmActors { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Actor>().ToTable("actor");
        modelBuilder.Entity<Film>().ToTable("film");
        modelBuilder.Entity<FilmActor>().ToTable("film_actor");
        modelBuilder.Entity<Category>().ToTable("category");
        modelBuilder.Entity<Customer>().ToTable("customer");
        modelBuilder.Entity<Rental>().ToTable("rental");
        modelBuilder.Entity<Store>().ToTable("store");
        modelBuilder.Entity<Inventory>().ToTable("inventory");


        // Mapeo de columnas de Film
        modelBuilder.Entity<Film>(entity =>
        {
            entity.Property(e => e.FilmId).HasColumnName("film_id");
            entity.Property(e => e.Title).HasColumnName("title").HasColumnType("varchar(255)");
            entity.Property(e => e.Description).HasColumnName("description").HasColumnType("text");
            entity.Property(e => e.ReleaseYear).HasColumnName("release_year").HasColumnType("varchar(4)");
            entity.Property(e => e.RentalDuration).HasColumnName("rental_duration").HasColumnType("tinyint");
            entity.Property(e => e.RentalRate).HasColumnName("rental_rate").HasColumnType("decimal(4,2)");
            entity.Property(e => e.Length).HasColumnName("length").HasColumnType("smallint");
            entity.Property(e => e.ReplacementCost).HasColumnName("replacement_cost").HasColumnType("decimal(5,2)");
            entity.Property(e => e.Rating).HasColumnName("rating").HasColumnType("varchar(10)");
            entity.Property(e => e.LanguageId).HasColumnName("language_id").HasColumnType("tinyint");
            entity.Property(e => e.OriginalLanguageId).HasColumnName("original_language_id").HasColumnType("tinyint");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });

        // Mapeo de columnas de Actor
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.Property(e => e.ActorId).HasColumnName("actor_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasColumnType("varchar(45)");
            entity.Property(e => e.LastName).HasColumnName("last_name").HasColumnType("varchar(45)");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });

        // Mapeo de FilmActor
        modelBuilder.Entity<FilmActor>(entity =>
        {
            entity.HasKey(fa => new { fa.ActorId, fa.FilmId }); // ← ORDEN CORRECTO
            entity.Property(fa => fa.FilmId).HasColumnName("film_id");
            entity.Property(fa => fa.ActorId).HasColumnName("actor_id");
            entity.Property(fa => fa.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.HasOne(fa => fa.Film)
                .WithMany(f => f.FilmActors)
                .HasForeignKey(fa => fa.FilmId);
            entity.HasOne(fa => fa.Actor)
                .WithMany(a => a.FilmActors)
                .HasForeignKey(fa => fa.ActorId);
        });

        // Mapeo de Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).HasColumnName("category_id").HasColumnType("tinyint");
            entity.Property(e => e.Name).HasColumnName("name").HasColumnType("varchar(25)");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });

        // Mapeo de Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasColumnType("varchar(45)");
            entity.Property(e => e.LastName).HasColumnName("last_name").HasColumnType("varchar(45)");
            entity.Property(e => e.Email).HasColumnName("email").HasColumnType("varchar(50)");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Active)
                  .HasColumnName("active")
                  .HasColumnType("char(1)")
                  // ESTO CONVIERTE EL '1' DE LA BD EN TRUE Y EL '0' EN FALSE AUTOMÁTICAMENTE
                  .HasConversion(
                      v => v ? "1" : "0", // DE BOOL A STRING (PARA GUARDAR)
                      v => v == "1"       // DE STRING A BOOL (PARA LEER)
                  );
            entity.Property(e => e.CreateDate).HasColumnName("create_date").HasColumnType("datetime");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
        });

        // Mapeo de Rental
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.RentalDate).HasColumnName("rental_date").HasColumnType("datetime");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date").HasColumnType("datetime");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });

        // Mapeo de Store
        modelBuilder.Entity<Store>(entity =>
        {
            entity.ToTable("store");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.ManagerStaffId).HasColumnName("manager_staff_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });

        // Mapeo de Inventory
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.FilmId).HasColumnName("film_id");
            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.LastUpdate).HasColumnName("last_update").HasColumnType("datetime");
            entity.HasOne(d => d.Film).WithMany().HasForeignKey(d => d.FilmId);
            entity.Property(e => e.Active).HasColumnName("active").HasDefaultValue(true);
        });
    }
}
