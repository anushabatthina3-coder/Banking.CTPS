namespace Banking.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}