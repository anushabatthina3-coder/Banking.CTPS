using Banking.Domain.Enums;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public Money Amount { get; private set; }
    public string? Reference { get; private set; }

    private Transaction() { }

    public Transaction(Guid accountId, TransactionType type, Money amount, string? reference = null)
    {
        AccountId = accountId;
        Type = type;
        Amount = amount;
        Status = TransactionStatus.Pending;
        Reference = reference;
    }

    public void MarkCompleted() => Status = TransactionStatus.Completed;
    public void MarkFailed() => Status = TransactionStatus.Failed;
}