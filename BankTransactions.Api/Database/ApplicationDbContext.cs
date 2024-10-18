using BankTransactions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTransactions.Api.Database;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
}