
namespace FinTrack.Core.Interfaces
{
    // Patrón de diseño: Adaptador
    public interface IUser
    {
        string Id { get; set; }
        string Email { get; set; }
        string UserName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
