using APP.DTO.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers.Identity;
[ApiController]
public class AccountController : ControllerBase
{ 

    public AccountController(ILogger<AccountController> logger)
    {
        
    }
    
    
    // [HttpPost]
    // public async Task<ActionResult<JWTResponse>> Login(
    //     []
    //     LoginInfo loginInfo,
    //     []
    //     int jwtExpiresInSeconds,
    //     []
    //     int refreshTokenExpiresInSeconds
    // )
    // {
    //     if () {}
    // }
}