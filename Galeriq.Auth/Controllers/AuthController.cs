using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Galeriq.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOpenIddictScopeManager scopeManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _scopeManager = scopeManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok();
    }

    // Token endpoint is handled by OpenIddict at /connect/token; show a helper for password grant testing
    [HttpPost("token-test")]
    public async Task<IActionResult> TokenTest([FromBody] TokenRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null) return Unauthorized();
        var valid = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!valid) return Unauthorized();

        // create claims principal
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        principal.SetScopes(new[] { OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email });

        var token = "(token is issued by OpenIddict at /connect/token)";
        return Ok(new { token, claims = principal.Claims.Select(c => new { c.Type, c.Value }) });
    }

    public record RegisterRequest(string Email, string Password);
    public record TokenRequest(string Email, string Password);
}
