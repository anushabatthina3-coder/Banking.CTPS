using Banking.Application.Abstractions;

namespace Banking.Application.Accounts;

public sealed class GetAccountBalanceQuery
{
    public string AccountNumber { get; set; } = null!;
}

public sealed class GetAccountBalanceHandler
{
    private readonly IAccountRepository _accounts;

    public GetAccountBalanceHandler(IAccountRepository accounts)
    {
        _accounts = accounts;
    }

    public async Task<AccountDto> Handle(GetAccountBalanceQuery query)
    {
        var account = await _accounts.GetByAccountNumberAsync(query.AccountNumber)
                      ?? throw new InvalidOperationException("Account not found.");

        return new AccountDto(
            account.Id,
            account.AccountNumber,
            account.OwnerName,
            account.Balance.Amount,
            account.Balance.Currency);
    }
}