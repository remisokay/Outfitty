using APP.DTO.v1.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers.Identity;
[ApiController]
public class AccountController : ControllerBase
{ 

    public AccountController(ILogger<AccountController> logger)
    {
        
    }
    
}