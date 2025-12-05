using Banking.Domain.Entities;

namespace Banking.Application.Abstractions;

public interface IAccountRepository
{
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task AddAsync(Account account);
}