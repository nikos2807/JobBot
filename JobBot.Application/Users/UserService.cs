using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;

namespace JobBot.Application.Users;

public class UserService(IUserRepository userRepository)
{
    public async Task<User> GetOrCreateAsync(
        long telegramId,
        string? username,
        string? firstName,
        string? lastName)
    {
        var existingUser = await userRepository.GetByTelegramIdAsync(telegramId);

        if (existingUser is not null)
            return existingUser;

        var user = new User
        {
            TelegramId = telegramId,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await userRepository.AddAsync(user);

        return user;
    }

    public Task<User?> GetByIdAsync(long id)
    {
        return userRepository.GetByIdAsync(id);
    }
}