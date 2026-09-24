using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using DevCom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevCom.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
