using DevCom.Application.DTOs.Devs;
using DevCom.Application.Interfaces;
using DevCom.Domain.Enums;

namespace DevCom.Application.Services;

public class DevProfileService(
    IUserRepository     userRepo,
    IFeedbackRepository feedbackRepo,
    IProjectRepository  projectRepo,
    ICacheService       cache)
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    public async Task<DevProfileResponse> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var cacheKey = $"dev_profile_{email.ToLowerInvariant()}";
        var cached   = await cache.GetAsync<DevProfileResponse>(cacheKey, ct);
        if (cached is not null) return cached;

        var user = await userRepo.GetByEmailAsync(email.ToLowerInvariant().Trim(), ct)
                   ?? throw new KeyNotFoundException("Desenvolvedor não encontrado.");

        if (user.Role != UserRole.Dev)
            throw new KeyNotFoundException("Perfil não encontrado.");

        var feedbacks         = await feedbackRepo.GetByDevAsync(user.Id, ct);
        var completedProjects = await projectRepo.GetCompletedByDevAsync(user.Id, ct);

        double rating = feedbacks.Count > 0
            ? Math.Round(feedbacks.Average(f => f.Stars), 1)
            : 0.0;

        var response = new DevProfileResponse
        {
            Name           = user.Name,
            Bio            = user.Bio,
            Rating         = rating,
            MemberSince    = user.CreatedAt,
            CompletedCount = completedProjects.Count,
            Skills         = user.Skills?.Select(s => s.Name).ToList() ?? new(),
            Feedbacks      = feedbacks.Select(f => new FeedbackDto
            {
                Stars   = f.Stars,
                Text    = f.Text,
                Company = f.Company
            }).ToList(),
            PastProjects = completedProjects.Select(p => new PastProjectDto
            {
                Title    = p.Title,
                Tags     = p.Tags?.Select(t => t.Name).ToList() ?? new(),
                Duration = p.Deadline
            }).ToList()
        };

        await cache.SetAsync(cacheKey, response, CacheTtl, ct);
        return response;
    }
}
