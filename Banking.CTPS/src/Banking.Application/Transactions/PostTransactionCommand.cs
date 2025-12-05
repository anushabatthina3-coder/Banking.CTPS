using Banking.Application.Abstractions;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.ValueObjects;

namespace Banking.Application.Transactions;

public sealed class PostTransactionCommand
{
    public string FromAccountNumber { get; set; } = null!;
    public string? ToAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

public sealed class PostTransactionHandler
{
    private readonly IAccountRepository _accounts;
    private readonly ITransactionRepository _transactions;
    private readonly IUnitOfWork _uow;

    public PostTransactionHandler(
        IAccountRepository accounts,
        ITransactionRepository transactions,
        IUnitOfWork uow)
    {
        _accounts = accounts;
        _transactions = transactions;
        _uow = uow;
    }

    public async Task<string> Handle(PostTransactionCommand cmd)
    {
        if (cmd.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        var amount = new Money(cmd.Amount);

        var from = await _accounts.GetByAccountNumberAsync(cmd.FromAccountNumber)
                   ?? throw new InvalidOperationException("From account not found.");

        Account? to = null;
        if (!string.IsNullOrWhiteSpace(cmd.ToAccountNumber))
        {
            to = await _accounts.GetByAccountNumberAsync(cmd.ToAccountNumber)
                 ?? throw new InvalidOperationException("To account not found.");
        }

        if (to is null)
        {
            from.Debit(amount);
            var tx = new Transaction(from.Id, TransactionType.Debit, amount, cmd.Reference);
            tx.MarkCompleted();
            from.AddTransaction(tx);
            await _transactions.AddAsync(tx);
        }
        else
        {
            from.Debit(amount);
            var debitTx = new Transaction(from.Id, TransactionType.Debit, amount, cmd.Reference);
            debitTx.MarkCompleted();
            from.AddTransaction(debitTx);
            await _transactions.AddAsync(debitTx);

            to.Credit(amount);
            var creditTx = new Transaction(to.Id, TransactionType.Credit, amount, cmd.Reference);
            creditTx.MarkCompleted();
            to.AddTransaction(creditTx);
            await _transactions.AddAsync(creditTx);
        }

        await _uow.SaveChangesAsync();
        return "Transaction successful";
    }
}