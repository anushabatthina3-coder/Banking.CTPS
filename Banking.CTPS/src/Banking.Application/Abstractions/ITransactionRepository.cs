using Banking.Domain.Entities;

namespace Banking.Application.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
}