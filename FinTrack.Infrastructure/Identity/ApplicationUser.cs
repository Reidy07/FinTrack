
using FinTrack.Core.Entities;
using FinTrack.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.Infrastructure.Identity
{
    // Patrón de diseño: Adaptador
    public class ApplicationUser: IdentityUser, IUser
    {
        // Implementación de IUser
        string IUser.Email
        {
            get => Email ?? string.Empty;
            set => Email = value;
        }

        string IUser.UserName
        {
            get => UserName ?? string.Empty;
            set => UserName = value;
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Conversión implícita (opcional pero útil)
        public static implicit operator User(ApplicationUser appUser)
        {
            return new User
            {
                Id = appUser.Id,
                Email = appUser.Email ?? string.Empty,
                UserName = appUser.UserName ?? string.Empty,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                CreatedAt = appUser.CreatedAt
            };
        }

    }
}
