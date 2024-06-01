using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using DayToDay.Models.DTO.User;
using DayToDay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserManager<UserModel> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public UserController(UserService userService, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser()
    {
        // check if the account data is valid
        // create new user instance
        // save in database 
        // return a token
        return Ok();
    }
    [HttpPost("LoginUser")]
    // public async Task<IActionResult> LoginUser([FromBody] LoginDTO model) {
    //     var user = await _userManager.FindByEmailAsync(model.Email);
    //     if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
    //     {
    //         Log.Information("Controller: Auth, Method: Signin, Message: User with the correct password found");
    //         var userRoles = await _userManager.GetRolesAsync(user);
    //         var authClaims = new List<Claim>
    //         {
    //             new Claim(ClaimTypes.Name, user.UserName),
    //             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //             new Claim(ClaimTypes.Email, user.Email),
    //             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //         };
    //         foreach (var userRole in userRoles)
    //         {
    //             authClaims.Add(new Claim(ClaimTypes.Role, userRole));
    //         }
    //
    //         var userData = new
    //         {
    //             id = user.Id,
    //             email = user.Email,
    //             username = user.UserName
    //         };
    //         var token = SetToken(authClaims);
    //         Log.Information("Controller: Auth, Method: Signin, Message: signed in Successfully");
    //         return Ok(new
    //         {
    //             token = new JwtSecurityTokenHandler().WriteToken(token),
    //             expiration = DateTime.UtcNow.AddHours(3),
    //             user = userData
    //         });
    //     }
    //     return Unauthorized(new {status = 401, message = "unauthorized"});
    // }
    private JwtSecurityToken SetToken(List<Claim> authClaims) {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(System.Environment.GetEnvironmentVariable("JWT_KEY")));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }
    
    
    
    
}