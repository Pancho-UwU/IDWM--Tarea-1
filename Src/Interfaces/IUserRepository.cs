using courses_dotnet_api.Src.Models;

namespace courses_dotnet_api.Src.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExistsByEmailAsync(string email);
    Task<bool> UserExistsByRutAsync(string rut);
    Task<User> GetUserByEmailAsync(string email); // Agregar este método

    Task<bool> SaveChangesAsync();
}
