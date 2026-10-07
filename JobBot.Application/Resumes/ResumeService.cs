using JobBot.Application.Abstractions.Repositories;
using JobBot.Domain.Entities;

namespace JobBot.Application.Resumes;

public class ResumeService(IResumeRepository resumeRepository)
{
    public Task<Resume?> GetActiveAsync(long userId)
    {
        return resumeRepository.GetActiveByUserIdAsync(userId);
    }

    public async Task<Resume> CreateAsync(
        long userId,
        string fileName,
        string? fileUrl,
        string? text)
    {
        var resume = new Resume
        {
            UserId = userId,
            FileName = fileName,
            FileUrl = fileUrl,
            Text = text,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await resumeRepository.AddAsync(resume);

        return resume;
    }
}