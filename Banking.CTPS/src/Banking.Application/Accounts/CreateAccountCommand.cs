using Banking.Application.Abstractions;
using Banking.Domain.Entities;
using Banking.Domain.ValueObjects;

namespace Banking.Application.Accounts;

public sealed class CreateAccountCommand
{
    public string AccountNumber { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public decimal OpeningBalance { get; set; }
    public string Currency { get; set; } = "USD";
}

public sealed class CreateAccountHandler
{
    private readonly IAccountRepository _accounts;
    private readonly IUnitOfWork _uow;

    public CreateAccountHandler(IAccountRepository accounts, IUnitOfWork uow)
    {
        _accounts = accounts;
        _uow = uow;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand command)
    {
        var existing = await _accounts.GetByAccountNumberAsync(command.AccountNumber);
        if (existing is not null)
            throw new InvalidOperationException("Account already exists.");

        var account = new Account(
            command.AccountNumber,
            command.OwnerName,
            new Money(command.OpeningBalance, command.Currency)
        );

        await _accounts.AddAsync(account);
        await _uow.SaveChangesAsync();

        return new AccountDto(
            account.Id,
            account.AccountNumber,
            account.OwnerName,
            account.Balance.Amount,
            account.Balance.Currency);
    }
}