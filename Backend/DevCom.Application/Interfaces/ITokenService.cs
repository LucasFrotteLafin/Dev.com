using DevCom.Domain.Entities;

namespace DevCom.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
