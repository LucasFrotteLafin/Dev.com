namespace DevCom.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public UserDto User { get; init; } = null!;
}

public class UserDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
