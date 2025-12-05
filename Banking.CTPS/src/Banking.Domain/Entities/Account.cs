using Banking.Domain.ValueObjects;

namespace Banking.Domain.Entities;

public class Account : BaseEntity
{
    public string AccountNumber { get; private set; } = null!;
    public string OwnerName { get; private set; } = null!;
    public Money Balance { get; private set; } = Money.Zero();

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private Account() { }

    public Account(string accountNumber, string ownerName, Money openingBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.");

        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("Owner name is required.");

        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = openingBalance;
    }

    public void Credit(Money amount)
    {
        Balance = Balance.Add(amount);
    }

    public void Debit(Money amount)
    {
        if (amount.Amount > Balance.Amount)
            throw new InvalidOperationException("Insufficient funds.");

        Balance = Balance.Subtract(amount);
    }

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
    }
}