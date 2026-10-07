using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;

namespace JobBot.Application.Search;

public class SearchFilterService(
    ISearchFilterRepository searchFilterRepository)
{
    public Task<SearchFilter?> GetAsync(long userId)
    {
        return searchFilterRepository.GetByUserIdAsync(userId);
    }

    public async Task<SearchFilter> CreateAsync(
        long userId,
        string? keywords,
        decimal? salaryFrom,
        decimal? salaryTo,
        string? location,
        bool remoteOnly,
        string? employmentType)
    {
        if (salaryFrom.HasValue &&
            salaryTo.HasValue &&
            salaryFrom > salaryTo)
        {
            throw new ArgumentException(
                "Минимальная зарплата не может быть больше максимальной.");
        }

        var filter = new SearchFilter
        {
            UserId = userId,
            Keywords = keywords,
            SalaryFrom = salaryFrom,
            SalaryTo = salaryTo,
            Location = location,
            RemoteOnly = remoteOnly,
            EmploymentType = employmentType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await searchFilterRepository.AddAsync(filter);

        return filter;
    }
}