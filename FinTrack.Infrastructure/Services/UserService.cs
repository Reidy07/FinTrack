using FinTrack.Core.Interfaces.Services;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetUserEmailAsync(string userId)
        {
            // Buscamos el correo en la tabla AspNetUsers
            var user = await _context.Users
                .AsNoTracking() // Hace que la consulta sea más rápida porque solo vamos a leer
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Email ?? string.Empty;
        }
    }
}