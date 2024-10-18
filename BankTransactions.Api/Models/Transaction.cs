namespace BankTransactions.Api.Models;

public sealed class Transaction(Guid senderId, Guid recipientId, decimal amount)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SenderId { get; private set; } = senderId;
    public Guid RecipientId { get; private set; } = recipientId;
    public decimal Amount { get; private set; } = SetAmount(amount);
    public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public void SetProccessed()
    {
        Status = TransactionStatus.Proccessed;
    }

    private static decimal SetAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Should be greater than zero", nameof(amount));

        return amount;
    }
}
