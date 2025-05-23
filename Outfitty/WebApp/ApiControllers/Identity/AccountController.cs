using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APP.DAL.EF;
using APP.DTO.v1;
using APP.DTO.v1.Identity;
using Asp.Versioning;
using BASE.Helpers;
using Domain.identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers.Identity;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{ 
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly Random _random = new Random();
    private readonly AppDbContext _context;
    
    private const string UserPassProblem = "User/Password problem";
    private const int RandomDelayMin = 500;
    private const int RandomDelayMax = 5000;
    
    private const string SettingsJwtPrefix = "JWTSecurity";
    private const string SettingsJwtKey = SettingsJwtPrefix + ":Key";
    private const string SettingsJwtIssuer = SettingsJwtPrefix + ":Issuer";
    private const string SettingsJwtAudience = SettingsJwtPrefix + ":Audience";
    private const string SettingsJwtExpiresInSeconds = SettingsJwtPrefix + ":ExpiresInSeconds";
    private const string SettingsJwtRefreshTokenExpiresInSeconds = SettingsJwtPrefix + ":RefreshTokenExpiresInSeconds";

    public AccountController(IConfiguration configuration, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager, ILogger<AccountController> logger, AppDbContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
    }
    
    // User authentication, returns JWT and refresh token
    
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Message), StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> Login(
        [FromBody] LoginInfo loginInfo,
        [FromQuery] int? jwtExpiresInSeconds,
        [FromQuery] int? refreshTokenExpiresInSeconds
    )
    {
        // verify user
        var appUser = await _userManager.FindByEmailAsync(loginInfo.Email);
        if (appUser == null)
        {
            _logger.LogWarning("WebApi login failed, email {} not found", loginInfo.Email);
            await Task.Delay(_random.Next(RandomDelayMin, RandomDelayMax));
            return NotFound(new Message(UserPassProblem));
        }

        // verify password
        var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginInfo.Password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("WebApi login failed, password {} for email {} was wrong", loginInfo.Password,
                loginInfo.Email);
            await Task.Delay(_random.Next(RandomDelayMin, RandomDelayMax));
            return NotFound(new Message(UserPassProblem));
        }
        
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        
        // clean up expired refresh tokens
        if (!_context.Database.ProviderName!.Contains("InMemory"))
        {
            var deletedRows = await _context
                .RefreshTokens
                .Where(t => t.UserId == appUser.Id && t.Expiration < DateTime.UtcNow)
                .ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {} expired refresh tokens", deletedRows);
        }
        else
        {
            //TODO: inMemory delete for testing
        }
        
        // create new token
        var refreshToken = new AppRefreshToken()
        {
            UserId = appUser.Id,
            Expiration = GetExpirationDateTime(refreshTokenExpiresInSeconds, SettingsJwtRefreshTokenExpiresInSeconds)
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // generating jwt
        var jwt = IdentityExtensions.GenerateJwt(
            claimsPrincipal.Claims,
            _configuration.GetValue<string>(SettingsJwtKey)!,
            _configuration.GetValue<string>(SettingsJwtIssuer)!,
            _configuration.GetValue<string>(SettingsJwtAudience)!,
            GetExpirationDateTime(jwtExpiresInSeconds, SettingsJwtExpiresInSeconds)
        );

        var responseData = new JwtResponse()
        {
            Jwt = jwt,
            RefreshToken = refreshToken.RefreshToken
        };

        return Ok(responseData);
    }
    
    
    // User registration, returns JWT and refresh token
    
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Message), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> Register(
        [FromBody] Register registerModel,
        [FromQuery] int? jwtExpiresInSeconds,
        [FromQuery] int? refreshTokenExpiresInSeconds
    )
    {
        var appUser = await _userManager.FindByEmailAsync(registerModel.Email);
        if (appUser != null)
        {
            _logger.LogWarning(" User {Email} already registered", registerModel.Email);
            return BadRequest(new Message("User already registered"));
        }

        var refreshToken = new AppRefreshToken()
        {
            Expiration = GetExpirationDateTime(refreshTokenExpiresInSeconds, SettingsJwtRefreshTokenExpiresInSeconds)
        };

        appUser = new AppUser()
        {
            Email = registerModel.Email,
            UserName = registerModel.Email,
            Username = registerModel.Username,
            Bio = registerModel.Bio,
            
            TotalOutfits = 0,
            TotalClothingItems = 0,

            RefreshTokens = new List<AppRefreshToken>()
            {
                refreshToken
            }
        };
        var result = await _userManager.CreateAsync(appUser, registerModel.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} created a new account with password", appUser.Email);

                await _userManager.AddClaimAsync(appUser, new Claim("Username", appUser.Username));
                await _userManager.AddClaimAsync(appUser, new Claim(ClaimTypes.Email, appUser.Email));
                // bio claim if it exists
                if (!string.IsNullOrEmpty(appUser.Bio))
                {
                    await _userManager.AddClaimAsync(appUser, new Claim("Bio", appUser.Bio));
                }
                await _userManager.AddClaimAsync(appUser, new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));
                
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
                var jwt = IdentityExtensions.GenerateJwt(
                    claimsPrincipal.Claims,
                    _configuration.GetValue<string>(SettingsJwtKey)!,
                    _configuration.GetValue<string>(SettingsJwtIssuer)!,
                    _configuration.GetValue<string>(SettingsJwtAudience)!,
                    GetExpirationDateTime(jwtExpiresInSeconds, SettingsJwtExpiresInSeconds)
                );
                _logger.LogInformation("WebApi registration successful. User {Email}", registerModel.Email);
                return Ok(new JwtResponse()
                {
                    Jwt = jwt,
                    RefreshToken = refreshToken.RefreshToken,
                });

        }

        var errors = result.Errors.Select(error => error.Description).ToList();
        return BadRequest(new Message() { Messages = errors });
    }
    
    
    // Renew JWT using refresh token
    
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Message), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> RenewRefreshToken(
        [FromBody] RefreshTokenModel refreshTokenModel,
        [FromQuery] int? jwtExpiresInSeconds,
        [FromQuery] int? refreshTokenExpiresInSeconds
    )
    {
        JwtSecurityToken jwtToken;
        
        // get user info from jwt
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(refreshTokenModel.Jwt);
            if (jwtToken == null)
            {
                return BadRequest(new Message("No token"));
            }
        }
        catch (Exception e)
        {
            return BadRequest(new Message($"Cant parse the token, {e.Message}"));
        }

        // validate jwt, ignore expiration date
        if (!IdentityExtensions.ValidateJwt(
                refreshTokenModel.Jwt,
                _configuration.GetValue<string>(SettingsJwtKey)!,
                _configuration.GetValue<string>(SettingsJwtIssuer)!,
                _configuration.GetValue<string>(SettingsJwtAudience)!
            ))
        {
            return BadRequest(new Message("JWT validation failed"));
        }

        var userEmail = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return BadRequest(new Message("No email in jwt"));
        }

        // get user and tokens
        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return NotFound(new Message($"User with email {userEmail} not found"));
        }

        // load and compare refresh tokens
        await _context.Entry(appUser).Collection(u => u.RefreshTokens!)
            .Query()
            .Where(x =>
                (x.RefreshToken == refreshTokenModel.RefreshToken && x.Expiration > DateTime.UtcNow) ||
                (x.PreviousRefreshToken == refreshTokenModel.RefreshToken &&
                 x.PreviousExpiration > DateTime.UtcNow)
            )
            .ToListAsync();

        if (appUser.RefreshTokens == null)
        {
            return Problem("RefreshTokens collection is null");
        }

        if (appUser.RefreshTokens.Count == 0)
        {
            return Problem("RefreshTokens collection is empty, no valid refresh tokens found");
        }

        if (appUser.RefreshTokens.Count != 1)
        {
            return Problem("More than one valid refresh token found.");
        }

        // get claims based user
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);

        // generate jwt
        var jwt = IdentityExtensions.GenerateJwt(
            claimsPrincipal.Claims,
            _configuration.GetValue<string>(SettingsJwtKey)!,
            _configuration.GetValue<string>(SettingsJwtIssuer)!,
            _configuration.GetValue<string>(SettingsJwtAudience)!,
            GetExpirationDateTime(jwtExpiresInSeconds, SettingsJwtExpiresInSeconds)
        );

        // make new refresh token, obsolete old ones
        var refreshToken = appUser.RefreshTokens.First();
        if (refreshToken.RefreshToken == refreshTokenModel.RefreshToken)
        {
            refreshToken.PreviousRefreshToken = refreshToken.RefreshToken;
            refreshToken.PreviousExpiration = DateTime.UtcNow.AddMinutes(1);

            refreshToken.RefreshToken = Guid.NewGuid().ToString();
            refreshToken.Expiration =
                GetExpirationDateTime(refreshTokenExpiresInSeconds, SettingsJwtRefreshTokenExpiresInSeconds);

            await _context.SaveChangesAsync();
        }

        var res = new JwtResponse()
        {
            Jwt = jwt,
            RefreshToken = refreshToken.RefreshToken,
        };

        return Ok(res);
    }
    
    
    // User logout
    
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Message), StatusCodes.Status404NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult> Logout([FromBody] LogoutInfo logout)
    {
        // delete the refresh token - so user is kicked out after jwt expiration
        // We do not invalidate the jwt on serverside - that would require pipeline modification and checking against db on every request
        // so client can actually continue to use the jwt until it expires (keep the jwt expiration time short ~1 min)

        var userId = User.GetUserId();
        var appUser = await _context.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            _logger.LogWarning("Logout failed - user with ID {UserId} not found", userId);
            return NotFound(new Message(UserPassProblem));
        }

        await _context.Entry(appUser)
            .Collection(u => u.RefreshTokens!)
            .Query()
            .Where(x =>
                (x.RefreshToken == logout.RefreshToken) ||
                (x.PreviousRefreshToken == logout.RefreshToken)
            )
            .ToListAsync();

        foreach (var appRefreshToken in appUser.RefreshTokens!)
        {
            _context.RefreshTokens.Remove(appRefreshToken);
        }

        var deleteCount = await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} logged out successfully. Deleted {Count} refresh tokens", userId, deleteCount);

        return Ok(new { TokenDeleteCount = deleteCount });
    }
    
    
    
    // helper methods
    private DateTime GetExpirationDateTime(int? expiresInSeconds, string settingsKey)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = int.MaxValue;
        expiresInSeconds = expiresInSeconds < _configuration.GetValue<int>(settingsKey)
            ? expiresInSeconds
            : _configuration.GetValue<int>(settingsKey);

        return DateTime.UtcNow.AddSeconds(expiresInSeconds ?? 60);
    }
}
