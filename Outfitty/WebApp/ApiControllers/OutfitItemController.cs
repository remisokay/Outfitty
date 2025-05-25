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
public class OutfitItemController : ControllerBase
{
    private readonly ILogger<OutfitItemController> _logger;
    private readonly IAppBll _bll;
    private readonly OutfitItemMapper _mapper = new OutfitItemMapper();

    public OutfitItemController(IAppBll bll, ILogger<OutfitItemController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all outfit items for an authenticated user
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> GetOutfitItems()
    {
        var userId = User.GetUserId();
        var outfitItems = await _bll.OutfitItems.AllAsync(userId);
        return Ok(outfitItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get outfit item by id
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OutfitItem>> GetOutfitItem(Guid id)
    {
        var userId = User.GetUserId();
        var outfitItem = await _bll.OutfitItems.FindAsync(id, userId);

        if (outfitItem == null) return NotFound(new Message("Outfit item not found"));
        
        return _mapper.Map(outfitItem)!;
    }
    
    
    // Get all specific outfit items
    [Produces("application/json")]
    [HttpGet("outfit/{outfitId:guid}")]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> GetItemsByOutfit(Guid outfitId)
    {
        var outfitItems = await _bll.OutfitItems.GetItemsByOutfitAsync(outfitId);
        return Ok(outfitItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get outfit items of specific type
    [Produces("application/json")]
    [HttpGet("outfit/{outfitId:guid}/type/{type}")]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> GetItemsByTypeInOutfit(Guid outfitId, ClothingType type)
    {
        var outfitItems = await _bll.OutfitItems.GetItemsByTypeInOutfitAsync(outfitId, type);
        return Ok(outfitItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get outer or inner layer items
    [Produces("application/json")]
    [HttpGet("outfit/{outfitId:guid}/outer")]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> GetOuterLayerItems(Guid outfitId)
    {
        var outfitItems = await _bll.OutfitItems.GetOuterLayerItemsAsync(outfitId);
        return Ok(outfitItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    [Produces("application/json")]
    [HttpGet("outfit/{outfitId:guid}/inner")]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> GetInnerLayerItems(Guid outfitId)
    {
        var outfitItems = await _bll.OutfitItems.GetInnerLayerItemsAsync(outfitId);
        return Ok(outfitItems.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Update outfit item
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutOutfitItem(Guid id, OutfitItem outfitItem)
    {
        if (id != outfitItem.Id)
            return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
        var bllOutfitItem = _mapper.Map(outfitItem);
        if (bllOutfitItem == null)
            return BadRequest(new Message("Invalid outfit item data"));
        
        await _bll.OutfitItems.UpdateAsync(bllOutfitItem, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Add item to the outfit
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<OutfitItem>> PostOutfitItem(OutfitItemCreate outfitItemCreate)
    {
        try
        {
            var createdItem = await _bll.OutfitItems.AddItemToOutfitAsync(
                outfitItemCreate.OutfitId, 
                outfitItemCreate.ClothingItemId, 
                outfitItemCreate.DisplayOrder);
            await _bll.SaveChangesAsync();

            var result = _mapper.Map(createdItem)!;
            return CreatedAtAction("GetOutfitItem", new { id = createdItem.Id }, result);
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
    
    
    // Update display order of the outfit items
    [Produces("application/json")]
    [HttpPut("{id:guid}/order/{newOrder:int}")]
    public async Task<ActionResult<OutfitItem>> UpdateDisplayOrder(Guid id, int newOrder)
    {
        try
        {
            var updatedItem = await _bll.OutfitItems.UpdateDisplayOrderAsync(id, newOrder);
            await _bll.SaveChangesAsync();
                
            return Ok(_mapper.Map(updatedItem)!);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    // Reorder items
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("outfit/{outfitId:guid}/reorder")]
    public async Task<ActionResult<IEnumerable<OutfitItem>>> ReorderItems(Guid outfitId, [FromBody] Dictionary<Guid, int> orderUpdates)
    {
        try
        {
            var reorderedItems = await _bll.OutfitItems.ReorderItemsAsync(outfitId, orderUpdates);
            await _bll.SaveChangesAsync();
                
            return Ok(reorderedItems.Select(x => _mapper.Map(x)!).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Get outfit with specific item
    [Produces("application/json")]
    [HttpGet("clothing/{clothingItemId:guid}/outfits")]
    public async Task<ActionResult<IEnumerable<Outfit>>> GetOutfitsContainingItem(Guid clothingItemId)
    {
        var outfits = await _bll.OutfitItems.GetOutfitsContainingClothingItemAsync(clothingItemId);
        var outfitMapper = new OutfitMapper();
        return Ok(outfits.Select(x => outfitMapper.Map(x)!).ToList());
    }
    
    
    // Delete outfit item
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOutfitItem(Guid id)
    {
        var userId = User.GetUserId();
            
        var outfitItem = await _bll.OutfitItems.FindAsync(id, userId);
        if (outfitItem == null)
            return NotFound(new Message("Outfit item not found"));

        await _bll.OutfitItems.RemoveAsync(id, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
}

/*
 * Some functions to add:
 *  recommendations, alternatives
 *  composition validation
 */