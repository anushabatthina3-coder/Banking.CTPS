namespace Banking.Application.Accounts;

public record AccountDto(
    Guid Id,
    string AccountNumber,
    string OwnerName,
    decimal Balance,
    string Currency
);