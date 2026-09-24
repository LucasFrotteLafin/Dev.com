using DevCom.Domain.Entities;

namespace DevCom.Application.Interfaces;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetByDevAsync(int devId, CancellationToken ct = default);
}
