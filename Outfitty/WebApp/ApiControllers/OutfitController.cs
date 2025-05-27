using APP.BLL.Contracts;
using APP.DTO.v1;
using APP.DTO.v1.Mappers;
using Asp.Versioning;
using BASE.Helpers;
using Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class OutfitController : ControllerBase
{
    private readonly ILogger<OutfitController> _logger;
    private readonly IAppBll _bll;
    private readonly OutfitMapper _mapper = new OutfitMapper();
    
    public OutfitController(IAppBll bll, ILogger<OutfitController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all outfits for an authenticated user
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetOutfits()
    {
        var userId = User.GetUserId();
        var outfits = await _bll.Outfits.GetUserOutfitsAsync(userId);
        return Ok(outfits.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get outfit by id
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Outfit>> GetOutfit(Guid id)
    {
        var userId = User.GetUserId();
        var outfit = await _bll.Outfits.GetOutfitWithItemsAsync(id, userId);

        if (outfit == null)
            return NotFound(new Message("Outfit not found"));
        
        return _mapper.Map(outfit)!;
    }
    
    
    // FILTERS TODO: check for functionality
    [Produces("application/json")]
    [HttpGet("season/{season}")]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetOutfitsBySeason(ClothingSeason season)
    {
        var userId = User.GetUserId();
        var outfits = await _bll.Outfits.GetOutfitsBySeasonAsync(userId, season);
        return Ok(outfits.Select(x => _mapper.Map(x)!).ToList());
    }
    
    [Produces("application/json")]
    [HttpGet("style/{style}")]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetOutfitsByStyle(ClothingStyle style)
    {
        var userId = User.GetUserId();
        var outfits = await _bll.Outfits.GetOutfitsByStyleAsync(userId, style);
        return Ok(outfits.Select(x => _mapper.Map(x)!).ToList());
    }
    
    // Get favourites
    [Produces("application/json")]
    [HttpGet("favorites")]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetFavoriteOutfits()
    {
        var userId = User.GetUserId();
        var outfits = await _bll.Outfits.GetFavoriteOutfitsAsync(userId);
        return Ok(outfits.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Update outfit
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutOutfit(Guid id, Outfit outfit)
    {
        if (id != outfit.Id)
            return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
        if (outfit.UserId != userId)
            return Forbid("Cannot modify other user's outfit");
        
        var bllOutfit = _mapper.Map(outfit);
        if (bllOutfit == null)
            return BadRequest(new Message("Invalid outfit data"));
        
        await _bll.Outfits.UpdateAsync(bllOutfit, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Create new outfit
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<Outfit>> PostOutfit(OutfitCreate outfitCreate)
    {
        var userId = User.GetUserId();
            
        // check that outfit belongs to user
        outfitCreate.UserId = userId;
            
        var bllOutfit = _mapper.Map(outfitCreate);
        _bll.Outfits.Add(bllOutfit, userId);
        await _bll.SaveChangesAsync();

        var createdOutfit = _mapper.Map(bllOutfit)!;
        return CreatedAtAction("GetOutfit", new { id = bllOutfit.Id }, createdOutfit);
    }
    
    
    // Add clothing item to the outfit
    [Produces("application/json")]
    [HttpPost("{id:guid}/items/{clothingItemId:guid}")]
    public async Task<ActionResult<Outfit>> AddClothingItemToOutfit(Guid id, Guid clothingItemId, [FromQuery] int displayOrder = 0)
    {
        try
        {
            var updatedOutfit = await _bll.Outfits.AddClothingItemToOutfitAsync(id, clothingItemId, displayOrder);
            await _bll.SaveChangesAsync();
                
            return Ok(_mapper.Map(updatedOutfit)!);
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
    
    
    // Remove clothing item from the outfit
    [Produces("application/json")]
    [HttpDelete("{id:guid}/items/{outfitItemId:guid}")]
    public async Task<IActionResult> RemoveClothingItemFromOutfit(Guid id, Guid outfitItemId)
    {
        try
        {
            await _bll.Outfits.RemoveClothingItemFromOutfitAsync(id, outfitItemId);
            await _bll.SaveChangesAsync();
                
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Delete outfit
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOutfit(Guid id)
    {
        var userId = User.GetUserId();
            
        // verify outfit exists and belongs to user
        var outfit = await _bll.Outfits.FindAsync(id, userId);
        if (outfit == null) return NotFound(new Message("Outfit not found"));
        
        await _bll.Outfits.RemoveAsync(id, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Assign image to clothing item
    [Produces("application/json")]
    [HttpPut("{id:guid}/image/{imageId:guid}")]
    public async Task<ActionResult<ClothingItem>> AssignImageToOutfit(Guid id, Guid imageId)
    {
            
        try
        {
            var updatedItem = await _bll.Outfits.AssignImageToOutfitAsync(id, imageId);
            await _bll.SaveChangesAsync();
                
            return Ok(_mapper.Map(updatedItem)!);
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
    
    
    /*
     * Some functions to add in the future:
     *  outfit completeness validation,
     *  style/season conflicts validation,
     *  recommendations
     */
}