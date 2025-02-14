using Microsoft.EntityFrameworkCore;
using Scores.Core.Models;

namespace Scores.Api.Data;

public class ScoreDbContext : DbContext
{
    public ScoreDbContext(DbContextOptions<ScoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<Score> Scores => Set<Score>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Score>()
            .HasIndex(s => new { s.FirstName, s.SecondName });
    }
}