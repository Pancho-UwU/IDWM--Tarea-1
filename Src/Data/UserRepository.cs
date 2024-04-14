using courses_dotnet_api.Src.Interfaces;
using courses_dotnet_api.Src.Models;
using Microsoft.EntityFrameworkCore;

namespace courses_dotnet_api.Src.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _dataContext;

    public UserRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _dataContext.Users.AnyAsync(user => user.Email == email);
    }

    public async Task<bool> UserExistsByRutAsync(string rut)
    {
        return await _dataContext.Users.AnyAsync(user => user.Rut == rut);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return 0 < await _dataContext.SaveChangesAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user == null)
        {
            throw new Exception("User not found");
        }
        return user;
    }
}
