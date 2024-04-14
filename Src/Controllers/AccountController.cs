using courses_dotnet_api.Src.DTOs.Account;
using courses_dotnet_api.Src.Interfaces;
using courses_dotnet_api.Src.Data.Migrations;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using courses_dotnet_api.Src.Data;
using courses_dotnet_api.Src.Services;
namespace courses_dotnet_api.Src.Controllers;

public class AccountController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    
    private readonly ITokenService _tokenService;

    public AccountController(IUserRepository userRepository, IAccountRepository accountRepository,ITokenService tokenService)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IResult> Register(RegisterDto registerDto)

    {
        if (
            await _userRepository.UserExistsByEmailAsync(registerDto.Email)
            || await _userRepository.UserExistsByRutAsync(registerDto.Rut)
        )
        {
            return TypedResults.BadRequest("User already exists");
        }

        await _accountRepository.AddAccountAsync(registerDto);

        if (!await _accountRepository.SaveChangesAsync())
        {
            return TypedResults.BadRequest("Failed to save user");
        }

        AccountDto? accountDto = await _accountRepository.GetAccountAsync(registerDto.Email);

        return TypedResults.Ok(accountDto);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        /*
        Si el usuario no escribio el correo o la contraseña, retorna un pensaque de que son necesarios
        */
        if(string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Email and password are required");
        }
        /*
        Si el usuauio no fue encontrado por el email, el email no existen, por lo tanto, no es valido.
        */
        if(!await _userRepository.UserExistsByEmailAsync(loginDto.Email))
        {
            return Unauthorized("Invalid Email");
        }
        bool contraseñaCorrecta = await _accountRepository.VerifyPasswordAsync(loginDto.Email, loginDto.Password);
        if(!contraseñaCorrecta )
        {
            return Unauthorized("Invalid authentication");
        }
        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
        var token = _tokenService.CreateToken(user.Rut);
        var accountDto = new AccountDto
        {
            Rut =user.Rut,
            Name = user.Name,
            Email = user.Email,
            Token = token
        };
        return Ok(accountDto);
    }

   
}
