using JobBot.Domain.Entities;

namespace JobBot.Application.Abstractions.Repositories;

public interface ISearchFilterRepository
{
    Task<SearchFilter?> GetByUserIdAsync(long userId);

    Task AddAsync(SearchFilter filter);

    Task UpdateAsync(SearchFilter filter);
}