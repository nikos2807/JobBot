using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(long id);

    Task<Payment?> GetByExternalIdAsync(string externalPaymentId);

    Task AddAsync(Payment payment);

    Task UpdateAsync(Payment payment);
}