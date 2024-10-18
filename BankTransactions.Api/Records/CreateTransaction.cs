namespace BankTransactions.Api.Records;

public record CreateTransaction(Guid SenderId, Guid RecipientId, decimal Amount);