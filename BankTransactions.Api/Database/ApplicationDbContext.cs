using BankTransactions.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTransactions.Api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
}