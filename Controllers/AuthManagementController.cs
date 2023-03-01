using Microsoft.AspNetCore.Mvc;
using ticktrax_backend.Models;
using ticktrax_backend.dtomodels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;

namespace ticktrax_backend.Controllers;

public class AuthManagementController : ControllerBase
{
    private readonly ILogger<AuthManagementController> _logger;
    protected readonly RoleManager<IdentityRole> roleManager;
    private IUserService userService;
    private UserManager<User> userManager;
    private IEmailService emailService;
    private SignInManager<User> signInManager;

    private readonly IConfiguration _config;

    

    public AuthManagementController(ILogger<AuthManagementController> logger, 
                                IUserService userService, 
                                UserManager<User> userManager, 
                                IEmailService emailService, 
                                SignInManager<User> signInManager,
                                IConfiguration configuration,
                                RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        this.userService = userService;
        this.userManager = userManager;
        this.emailService = emailService;
        this.signInManager = signInManager;
        this.roleManager = roleManager;
        _config = configuration;
    }

    private async Task<List<Claim>> GetValidClaims(User user)
    {

        IdentityOptions options = new IdentityOptions();
        
        var claims = new List<Claim>
        {
            new Claim("id", user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
            new Claim(options.ClaimsIdentity.UserIdClaimType, user.UserName),
        };

        var userClaims = await userManager.GetClaimsAsync(user);
        var userRoles = await userManager.GetRolesAsync(user);

        claims.AddRange(userClaims);

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
            var role = await roleManager.FindByNameAsync(userRole);
            if(role!=null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(role);
                foreach(Claim roleClaim in roleClaims)
                {
                    claims.Add(roleClaim);
                }
            }
        }

        return claims;

    }

    
}