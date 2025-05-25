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
public class WardrobeController : ControllerBase
{
    private readonly ILogger<WardrobeController> _logger;
    private readonly IAppBll _bll;
    private readonly WardrobeMapper _mapper = new WardrobeMapper();

    public WardrobeController(IAppBll bll, ILogger<WardrobeController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all wardrobes
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Wardrobe>>> GetWardrobes()
    {
        var userId = User.GetUserId();
        var wardrobes = await _bll.Wardrobes.GetUserWardrobesAsync(userId);
        return Ok(wardrobes.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Wardrobe by ID
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Wardrobe>> GetWardrobe(Guid id)
    {
        var userId = User.GetUserId();
        var wardrobe = await _bll.Wardrobes.FindAsync(id, userId);

        if (wardrobe == null) return NotFound(new Message("Wardrobe not found"));

        return _mapper.Map(wardrobe)!;
    }
    
    
    // Wardrobe with items (useless?)
    [Produces("application/json")]
    [HttpGet("{id:guid}/items")]
    public async Task<ActionResult<Wardrobe>> GetWardrobeWithItems(Guid id)
    {
        var userId = User.GetUserId();
        var wardrobe = await _bll.Wardrobes.GetWardrobeWithItemsAsync(id, userId);

        if (wardrobe == null) return NotFound(new Message("Wardrobe not found"));
        
        return _mapper.Map(wardrobe)!;
    }
    
    
    // FILTERING
    [Produces("application/json")]
    [HttpGet("{id:guid}/items/type/{type}")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetItemsByType(Guid id, ClothingType type)
    {
        var clothingItems = await _bll.Wardrobes.GetItemsByTypeAsync(id, type);
        var clothingItemMapper = new ClothingItemMapper();
        return Ok(clothingItems.Select(x => clothingItemMapper.Map(x)!).ToList());
    }
    
    [Produces("application/json")]
    [HttpGet("{id:guid}/items/season/{season}")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetItemsBySeason(Guid id, ClothingSeason season)
    {
        var clothingItems = await _bll.Wardrobes.GetItemsBySeasonAsync(id, season);
        var clothingItemMapper = new ClothingItemMapper();
        return Ok(clothingItems.Select(x => clothingItemMapper.Map(x)!).ToList());
    }
    
    [Produces("application/json")]
    [HttpGet("{id:guid}/items/style/{style}")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetItemsByStyle(Guid id, ClothingStyle style)
    {
        var clothingItems = await _bll.Wardrobes.GetItemsByStyleAsync(id, style);
        var clothingItemMapper = new ClothingItemMapper();
        return Ok(clothingItems.Select(x => clothingItemMapper.Map(x)!).ToList());
    }
    
    [Produces("application/json")]
    [HttpGet("{id:guid}/items/color/{color}")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetItemsByColor(Guid id, ClothingColor color)
    {
        var clothingItems = await _bll.Wardrobes.GetItemsByColorAsync(id, color);
        var clothingItemMapper = new ClothingItemMapper();
        return Ok(clothingItems.Select(x => clothingItemMapper.Map(x)!).ToList());
    }
    
    
    // Get statistics
    [Produces("application/json")]
    [HttpGet("{id:guid}/statistics")]
    public async Task<ActionResult<Dictionary<ClothingType, int>>> GetWardrobeStatistics(Guid id)
    {
        var statistics = await _bll.Wardrobes.GetWardrobeStatisticsAsync(id);
        return Ok(statistics);
    }
    
    
    // Update wardrobe
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutWardrobe(Guid id, Wardrobe wardrobe)
    {
        if (id != wardrobe.Id) return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
        if (wardrobe.UserId != userId) return Forbid("Cannot modify other user's wardrobe");
        
        var bllWardrobe = _mapper.Map(wardrobe);
        if (bllWardrobe == null) return BadRequest(new Message("Invalid wardrobe data"));
        
        await _bll.Wardrobes.UpdateAsync(bllWardrobe, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Create new wardrobe
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<Wardrobe>> PostWardrobe(WardrobeCreate wardrobeCreate)
    {
        var userId = User.GetUserId();
        
        wardrobeCreate.UserId = userId;
            
        var bllWardrobe = _mapper.Map(wardrobeCreate);
        _bll.Wardrobes.Add(bllWardrobe, userId);
        await _bll.SaveChangesAsync();

        var createdWardrobe = _mapper.Map(bllWardrobe)!;
        return CreatedAtAction("GetWardrobe", new { id = bllWardrobe.Id }, createdWardrobe);
    }
    
    
    // Delete the wardrobe
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWardrobe(Guid id)
    {
        var userId = User.GetUserId();
            
        // Verify wardrobe exists and belongs to user
        var wardrobe = await _bll.Wardrobes.FindAsync(id, userId);
        if (wardrobe == null)
        {
            return NotFound(new Message("Wardrobe not found"));
        }

        // Check if wardrobe has items
        var wardrobeWithItems = await _bll.Wardrobes.GetWardrobeWithItemsAsync(id, userId);
        if (wardrobeWithItems?.ItemCount > 0)
        {
            return BadRequest(new Message("Cannot delete wardrobe that contains clothing items. Remove all items first."));
        }

        await _bll.Wardrobes.RemoveAsync(id, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Add clothing item to wardrobe
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost("{id:guid}/items")]
    public async Task<ActionResult<ClothingItem>> AddClothingItemToWardrobe(Guid id, ClothingItemCreate clothingItemCreate)
    {
        var userId = User.GetUserId();
            
        // Verify wardrobe exists and belongs to user
        var wardrobe = await _bll.Wardrobes.FindAsync(id, userId);
        if (wardrobe == null)
        {
            return NotFound(new Message("Wardrobe not found"));
        }

        try
        {
            // Set the wardrobe ID
            clothingItemCreate.WardrobeId = id;
                
            var clothingItemMapper = new ClothingItemMapper();
            var bllClothingItem = clothingItemMapper.Map(clothingItemCreate);
                
            var addedItem = await _bll.Wardrobes.AddClothingItemToWardrobeAsync(id, bllClothingItem);
            await _bll.SaveChangesAsync();
                
            var result = clothingItemMapper.Map(addedItem)!;
            return CreatedAtAction("GetClothingItem", "ClothingItem", new { id = addedItem.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Remove clothing item 
    [Produces("application/json")]
    [HttpDelete("{id:guid}/items/{clothingItemId:guid}")]
    public async Task<IActionResult> RemoveClothingItemFromWardrobe(Guid id, Guid clothingItemId)
    {
        try
        {
            await _bll.Wardrobes.RemoveClothingItemFromWardrobeAsync(id, clothingItemId);
            await _bll.SaveChangesAsync();
                
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
}