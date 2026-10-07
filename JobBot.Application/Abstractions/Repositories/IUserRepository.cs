using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);

    Task<User?> GetByTelegramIdAsync(long telegramId);

    Task AddAsync(User user);

    Task UpdateAsync(User user);
}