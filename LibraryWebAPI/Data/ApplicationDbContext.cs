// LibraryWebAPI/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Models; // Import your Models namespace
using BCrypt.Net; // Make sure this is present for BCrypt.Net.BCrypt

namespace LibraryWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSets for each of your models
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Borrower> Borrowers { get; set; } = null!;
        public DbSet<Loan> Loans { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and any specific model constraints if needed

            // Example: Configure unique ISBN for Book
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            // Example: Configure unique Email for Borrower
            modelBuilder.Entity<Borrower>()
                .HasIndex(b => b.Email)
                .IsUnique();

            // --- ADD THIS LINE FOR UNIQUE USERNAME ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            // ----------------------------------------

            // Configure Loan relationships
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Borrower)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BorrowerId);

            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    // IMPORTANT: PASTE YOUR STATIC BCrypt HASH FOR "adminpass" HERE
                    // Example: "PasswordHash = "$2a$11$YOUR_GENERATED_STATIC_HASH_HERE","
                    PasswordHash = "$2a$11$fERxXMKf5YvYrE1NRFnSduKCSxK298OlvXz7C9Pv2va/Dt2lX5OOW"
                }
            );
        }
    }
}