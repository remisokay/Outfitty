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
public class ClothingItemController : ControllerBase
{
    private readonly ILogger<ClothingItemController> _logger;
    private readonly IAppBll _bll;
    private readonly ClothingItemMapper _mapper = new ClothingItemMapper();

    public ClothingItemController(IAppBll bll, ILogger<ClothingItemController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all clothing items
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetClothingItems()
    {
        var userId = User.GetUserId();
        var clothingItems = await _bll.ClothingItems.GetUserClothingItemsAsync(userId);
        return Ok(clothingItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get clothing item by ID
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClothingItem>> GetClothingItem(Guid id)
    {
        var userId = User.GetUserId();
        var clothingItem = await _bll.ClothingItems.FindAsync(id, userId);

        if (clothingItem == null)
            return NotFound(new Message("Clothing item not found"));
        
        return _mapper.Map(clothingItem)!;
    }
    
    
    // Filtering clothing items
    [Produces("application/json")]
    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> FilterClothingItems(
        [FromQuery] ClothingType? type = null,
        [FromQuery] ClothingStyle? style = null,
        [FromQuery] ClothingSeason? season = null,
        [FromQuery] ClothingColor? color = null)
    {
        var userId = User.GetUserId();
        var clothingItems = await _bll.ClothingItems.FilterItemsAsync(userId, type, style, season, color);
        return Ok(clothingItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    //  Get clothing items by wardrobe
    [Produces("application/json")]
    [HttpGet("wardrobe/{wardrobeId:guid}")]
    public async Task<ActionResult<IEnumerable<ClothingItem>>> GetClothingItemsByWardrobe(Guid wardrobeId)
    {
        var userId = User.GetUserId();
        var clothingItems = await _bll.ClothingItems.GetClothingItemsByWardrobeAsync(wardrobeId, userId);
        return Ok(clothingItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get outfits with particular clothing item
    [Produces("application/json")]
    [HttpGet("{id:guid}/outfits")]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetOutfitsContainingItem(Guid id)
    {
        var outfits = await _bll.ClothingItems.GetOutfitsContainingItemAsync(id);
        var outfitMapper = new OutfitMapper();
        return Ok(outfits.Select(x => outfitMapper.Map(x)!).ToList());
    }
    
    
    // Get usage count for clothing item
    [Produces("application/json")]
    [HttpGet("{id:guid}/usage")]
    public async Task<ActionResult<object>> GetItemUsage(Guid id)
    {
        var userId = User.GetUserId();
            
        // item must belong to user
        var item = await _bll.ClothingItems.FindAsync(id, userId);
        if (item == null)
            return NotFound(new Message("Clothing item not found"));
        
        var usageCount = await _bll.ClothingItems.GetItemUsageCountAsync(id);
        var outfits = await _bll.ClothingItems.GetOutfitsContainingItemAsync(id);
            
        var usage = new
        {
            ItemId = id,
            UsageCount = usageCount,
            OutfitNames = outfits.Select(o => o.Name).ToList(),
            IsFrequentlyUsed = usageCount > 3,
            IsUnused = usageCount == 0
        };

        return Ok(usage);
    }
    
    
    // Update clothing item
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutClothingItem(Guid id, ClothingItem clothingItem)
    {
        if (id != clothingItem.Id)
            return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
        
        var existingItem = await _bll.ClothingItems.FindAsync(id, userId);
        if (existingItem == null)
            return NotFound(new Message("Clothing item not found"));
        
        var bllClothingItem = _mapper.Map(clothingItem);
        if (bllClothingItem == null)
            return BadRequest(new Message("Invalid clothing item data"));
        
        await _bll.ClothingItems.UpdateAsync(bllClothingItem, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Create new clothing item
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<ClothingItem>> PostClothingItem(ClothingItemCreate clothingItemCreate)
    {
        var userId = User.GetUserId();
        
        var wardrobe = await _bll.Wardrobes.FindAsync(clothingItemCreate.WardrobeId, userId);
        if (wardrobe == null)
            return BadRequest(new Message("Wardrobe not found or does not belong to you"));
        
        var bllClothingItem = _mapper.Map(clothingItemCreate);
        _bll.ClothingItems.Add(bllClothingItem, userId);
        await _bll.SaveChangesAsync();

        var createdItem = _mapper.Map(bllClothingItem)!;
        return CreatedAtAction("GetClothingItem", new { id = bllClothingItem.Id }, createdItem);
    }
    
    
    // Delete clothing item
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClothingItem(Guid id)
    {
        var userId = User.GetUserId();
            
        try
        {
            var success = await _bll.ClothingItems.DeleteClothingItemAsync(id, userId);
            if (!success) return NotFound(new Message("Clothing item not found"));

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Assign image to clothing item
    [Produces("application/json")]
    [HttpPut("{id:guid}/image/{imageId:guid}")]
    public async Task<ActionResult<ClothingItem>> AssignImageToClothingItem(Guid id, Guid imageId)
    {
            
        try
        {
            var updatedItem = await _bll.ClothingItems.AssignImageToClothingItemAsync(id, imageId);
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
    
    
}