using APP.BLL.Contracts;
using APP.DTO.v1;
using APP.DTO.v1.Mappers;
using Asp.Versioning;
using BASE.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class FavouriteController : ControllerBase
{
    private readonly ILogger<FavouriteController> _logger;
    private readonly IAppBll _bll;
    private readonly FavouriteMapper _mapper = new FavouriteMapper();

    public FavouriteController(IAppBll bll, ILogger<FavouriteController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all favourites for user
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Favourite>>> GetFavourites()
    {
        var userId = User.GetUserId();
        var favourites = await _bll.Favourites.GetUserFavoritesAsync(userId);
        return Ok(favourites.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get favourite by id
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Favourite>> GetFavourite(Guid id)
    {
        var userId = User.GetUserId();
        var favourite = await _bll.Favourites.FindAsync(id, userId);

        if (favourite == null)
            return NotFound(new Message("Favourite not found"));

        return _mapper.Map(favourite)!;
    }
    
    
    // Check if is favourite
    [Produces("application/json")]
    [HttpGet("check/{outfitId:guid}")]
    public async Task<ActionResult<object>> CheckFavouriteStatus(Guid outfitId)
    {
        var userId = User.GetUserId();
        var isFavourite = await _bll.Favourites.IsOutfitFavoriteAsync(userId, outfitId);
            
        return Ok(new
        {
            OutfitId = outfitId,
            IsFavourite = isFavourite
        });
    }
    
    
    // Toggle favourite status if an outfit
    [Produces("application/json")]
    [HttpPost("toggle/{outfitId:guid}")]
    public async Task<ActionResult<object>> ToggleFavourite(Guid outfitId)
    {
        var userId = User.GetUserId();
            
        try
        {
            var isNowFavourited = await _bll.Favourites.ToggleFavoriteAsync(userId, outfitId);
            await _bll.SaveChangesAsync();
                
            return Ok(new
            {
                OutfitId = outfitId,
                IsFavourite = isNowFavourited,
                Action = isNowFavourited ? "Added to favourites" : "Removed from favourites"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Add to favourites
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<Favourite>> PostFavourite(FavouriteCreate favouriteCreate)
    {
        var userId = User.GetUserId();
        favouriteCreate.UserId = userId;
            
        try
        {
            var addedFavourite = await _bll.Favourites.AddToFavoritesAsync(userId, favouriteCreate.OutfitId);
            await _bll.SaveChangesAsync();

            var result = _mapper.Map(addedFavourite)!;
            return CreatedAtAction("GetFavourite", new { id = addedFavourite.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new Message(ex.Message));
        }
    }
    
    
    // Remove from favourites
    [Produces("application/json")]
    [HttpDelete("outfit/{outfitId:guid}")]
    public async Task<IActionResult> RemoveFavouriteByOutfit(Guid outfitId)
    {
        var userId = User.GetUserId();
            
        var success = await _bll.Favourites.RemoveFromFavoritesAsync(userId, outfitId);
        if (!success) return NotFound(new Message("Favourite not found"));

        await _bll.SaveChangesAsync();
        return NoContent();
    }
    
    
    // Delete favourite by id
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFavourite(Guid id)
    {
        var userId = User.GetUserId();
        
        var favourite = await _bll.Favourites.FindAsync(id, userId);
        if (favourite == null) return NotFound(new Message("Favourite not found"));
        
        await _bll.Favourites.RemoveAsync(id, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
}