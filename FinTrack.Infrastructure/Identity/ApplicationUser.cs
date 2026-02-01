
using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.Infrastructure.Identity
{
    // Patrón de diseño: Adaptador
    public class ApplicationUser: IdentityUser, IUser
    {
        // Implementación de IUser
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Conversión implícita (opcional pero útil)
        public static implicit operator User(ApplicationUser appUser)
        {
            return new User
            {
                Id = appUser.Id,
                Email = appUser.Email,
                UserName = appUser.UserName,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                CreatedAt = appUser.CreatedAt
            };
        }
    }
}
