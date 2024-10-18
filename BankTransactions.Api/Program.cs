using BankTransactions.Api.Database;
using BankTransactions.Api.Models;
using BankTransactions.Api.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("transactionsDb"));

builder.Services.AddScoped<ApplicationDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/transactions", async ([FromServices] ApplicationDbContext applicationDbContext) =>
{
    return await applicationDbContext.Transactions.AsNoTracking().ToListAsync();
})
.Produces(StatusCodes.Status200OK)
.WithName("GetTransactions")
.WithOpenApi();

app.MapGet("/transactions/{id}", async ([FromRoute] Guid Id, [FromServices] ApplicationDbContext applicationDbContext) =>
{
    var transaction = await GetTransactionById(Id, applicationDbContext);

    if (transaction == null)
        return HttpErrorResult.NotFound();

    return Results.Ok(transaction);
})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound, typeof(HttpErrorResult))
.WithName("GetTransaction")
.WithOpenApi();

app.MapPost("/transactions", async ([FromBody] CreateTransaction record, [FromServices] ApplicationDbContext applicationDbContext) =>
{
    try
    {
        if (applicationDbContext.Transactions.Any(x =>
        x.SenderId == record.SenderId &&
        x.RecipientId == record.RecipientId &&
        x.Amount == record.Amount &&
        x.CreatedAt >= DateTime.UtcNow.AddSeconds(-15)))
            return HttpErrorResult.BadRequest("Transaction duplicated");

        var transaction = new Transaction(record.SenderId, record.RecipientId, record.Amount);

        applicationDbContext.Transactions.Add(transaction);
        await applicationDbContext.SaveChangesAsync();

        return Results.Created($"/transactions/{transaction.Id}", transaction);
    }
    catch (ArgumentException ex)
    {
        return HttpErrorResult.BadRequest(ex.Message);
    }
})
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest, typeof(HttpErrorResult))
.WithName("CreateTransaction")
.WithOpenApi();

app.MapPatch("/transactions/{id}/proccess", async ([FromRoute] Guid Id, [FromServices] ApplicationDbContext applicationDbContext) =>
{
    var transaction = await GetTransactionById(Id, applicationDbContext);

    if (transaction == null)
        return HttpErrorResult.NotFound();

    transaction.SetProccessed();

    applicationDbContext.Transactions.Update(transaction);
    await applicationDbContext.SaveChangesAsync();
    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound, typeof(HttpErrorResult))
.WithName("ProcessTransaction")
.WithOpenApi();

app.Run();

static async Task<Transaction?> GetTransactionById(Guid Id, ApplicationDbContext applicationDbContext)
{
    return await applicationDbContext.Transactions.FindAsync(Id);
}