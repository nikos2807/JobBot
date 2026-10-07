using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;
using JobBot.Domain.Enums;

namespace JobBot.Application.JobApplications;

public class JobApplicationService(
    IApplicationRepository applicationRepository,
    ISubscriptionRepository subscriptionRepository)
{
    public Task<JobApplication?> GetByIdAsync(long id)
    {
        return applicationRepository.GetByIdAsync(id);
    }

    public Task<JobApplication?> GetByUserAndVacancyAsync(
        long userId,
        long vacancyId)
    {
        return applicationRepository.GetByUserAndVacancyAsync(
            userId,
            vacancyId);
    }

    public async Task<JobApplication> CreateAsync(
        long userId,
        long vacancyId)
    {
        var existingApplication =
            await applicationRepository.GetByUserAndVacancyAsync(
                userId,
                vacancyId);

        if (existingApplication is not null)
            return existingApplication;

        var subscription =
            await subscriptionRepository.GetActiveByUserIdAsync(userId);

        if (subscription is null ||
            subscription.Status != SubscriptionStatus.Active ||
            subscription.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "У пользователя нет активной подписки.");
        }

        var applicationsToday =
            await applicationRepository.CountTodayByUserIdAsync(userId);

        if (applicationsToday >= 100)
        {
            throw new InvalidOperationException(
                "Достигнут дневной лимит откликов.");
        }

        var application = new JobApplication
        {
            UserId = userId,
            VacancyId = vacancyId,
            Status = ApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await applicationRepository.AddAsync(application);

        return application;
    }

    public async Task MarkAsSentAsync(
        long id,
        string? externalApplicationId = null)
    {
        var application =
            await applicationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"Отклик с ID {id} не найден.");

        application.Status = ApplicationStatus.Sent;
        application.ExternalApplicationId = externalApplicationId;
        application.SentAt = DateTime.UtcNow;
        application.UpdatedAt = DateTime.UtcNow;

        await applicationRepository.UpdateAsync(application);
    }

    public async Task MarkAsFailedAsync(
        long id,
        string errorMessage)
    {
        var application =
            await applicationRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(
                $"Отклик с ID {id} не найден.");

        application.Status = ApplicationStatus.Failed;
        application.ErrorMessage = errorMessage;
        application.UpdatedAt = DateTime.UtcNow;

        await applicationRepository.UpdateAsync(application);
    }
}