using DevCom.Application.Interfaces;

namespace DevCom.Infrastructure.Security;

/// <summary>
/// Criptografa senhas com BCrypt work-factor 12.
/// Nunca armazena a senha em texto plano.
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plainText) =>
        BCrypt.Net.BCrypt.HashPassword(plainText, WorkFactor);

    public bool Verify(string plainText, string hash) =>
        BCrypt.Net.BCrypt.Verify(plainText, hash);
}
