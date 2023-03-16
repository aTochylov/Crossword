using CrosswordCreator.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrosswordCreator.Data
{
    public class CrosswordDbContext : DbContext
    {
        public DbSet<Crossword> Crosswords { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Row> Rows { get; set; }

        public CrosswordDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Crossword>().HasIndex(c => c.Code).IsUnique();

            modelBuilder.Entity<Row>()
                .HasOne<Crossword>(r => r.Crossword)
                .WithMany(c => c.Rows)
                .HasForeignKey(q => q.CrosswordId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Question>()
                .HasOne<Crossword>(q => q.Crossword)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.CrosswordId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Answer>()
                .HasOne<Question>(a => a.Question)
                .WithOne(q => q.Answer)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
