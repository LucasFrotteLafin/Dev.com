using DevCom.Application.DTOs.Auth;
using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;

namespace DevCom.Application.Services;

public class AuthService(
    IUserRepository userRepo,
    IPasswordHasher hasher,
    ITokenService   tokens)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        bool emailTaken = await userRepo.ExistsByEmailAsync(request.Email.ToLowerInvariant(), ct);
        if (emailTaken)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var user = new User
        {
            Name         = request.Name.Trim(),
            Email        = request.Email.ToLowerInvariant().Trim(),
            PasswordHash = hasher.Hash(request.Password),
            Role         = request.Role
        };

        await userRepo.AddAsync(user, ct);
        await userRepo.SaveChangesAsync(ct);

        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepo.GetByEmailAsync(request.Email.ToLowerInvariant().Trim(), ct);

        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-mail ou senha incorretos.");

        return BuildResponse(user);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var user = await userRepo.GetByEmailAsync(request.Email.ToLowerInvariant().Trim(), ct)
                   ?? throw new KeyNotFoundException("Usuário não encontrado.");

        user.PasswordHash = hasher.Hash(request.NewPassword);
        await userRepo.SaveChangesAsync(ct);
    }

    private AuthResponse BuildResponse(User user) => new()
    {
        Token = tokens.GenerateToken(user),
        User  = new UserDto
        {
            Id        = user.Id,
            Name      = user.Name,
            Email     = user.Email,
            Role      = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        }
    };
}
