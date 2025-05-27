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
public class ImageMetadataController : ControllerBase
{
    private readonly ILogger<ImageMetadataController> _logger;
    private readonly IAppBll _bll;
    private readonly ImageMetadataMapper _mapper = new ImageMetadataMapper();

    public ImageMetadataController(IAppBll bll, ILogger<ImageMetadataController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all images
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImageMetadata>>> GetImages()
    {
        var userId = User.GetUserId();
        var images = await _bll.Images.GetUserImagesAsync(userId);
        return Ok(images.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get image data by id
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImageMetadata>> GetImage(Guid id)
    {
        var userId = User.GetUserId();
        var image = await _bll.Images.FindAsync(id, userId);

        if (image == null)
            return NotFound(new Message("Image not found"));
        
        return _mapper.Map(image)!;
    }
    
    
    // Get image file data
    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> GetImageFile(Guid id)
    {
        var userId = User.GetUserId();
        
        var imageMetadata = await _bll.Images.FindAsync(id, userId);
        if (imageMetadata == null) return NotFound();
        
        var imageStream = await _bll.Images.GetImageDataAsync(id);
        if (imageStream == null) return NotFound();

        return File(imageStream, imageMetadata.ContentType, imageMetadata.OriginalFileName);
    }
    
    
    // Get image public url
    [Produces("application/json")]
    [HttpGet("{id:guid}/url")]
    public async Task<ActionResult<object>> GetImageUrl(Guid id)
    {
        var userId = User.GetUserId();
        
        var imageMetadata = await _bll.Images.FindAsync(id, userId);
        if (imageMetadata == null)
        {
            return NotFound(new Message("Image not found"));
        }

        var publicUrl = await _bll.Images.GetPublicUrlAsync(id);
            
        return Ok(new
        {
            ImageId = id,
            PublicUrl = publicUrl,
            imageMetadata.OriginalFileName,
            imageMetadata.ContentType
        });
    }
    
    
    // Get user's profile image
    [Produces("application/json")]
    [HttpGet("profile")]
    public async Task<ActionResult<ImageMetadata>> GetProfileImage()
    {
        var userId = User.GetUserId();
        var profileImage = await _bll.Images.GetProfileImageAsync(userId);

        if (profileImage == null) return NotFound(new Message("Profile image not found"));

        return _mapper.Map(profileImage)!;
    }
    
    
    // Get image for a specific item
    [Produces("application/json")]
    [HttpGet("clothing/{clothingItemId:guid}")]
    public async Task<ActionResult<ImageMetadata>> GetClothingItemImage(Guid clothingItemId)
    {
        var image = await _bll.Images.GetClothingItemImageAsync(clothingItemId);

        if (image == null) return NotFound(new Message("Image not found for clothing item"));

        return _mapper.Map(image)!;
    }
    
    // Get image for a specific outfit
    [Produces("application/json")]
    [HttpGet("outfit/{outfitId:guid}")]
    public async Task<ActionResult<ImageMetadata>> GetOutfitImage(Guid outfitId)
    {
        var image = await _bll.Images.GetOutfitImageAsync(outfitId);

        if (image == null) return NotFound(new Message("Image not found for outfit"));

        return _mapper.Map(image)!;
    }
    
    
    // Get all images for clothing items
    [Produces("application/json")]
    [HttpGet("wardrobe/{wardrobeId:guid}")]
    public async Task<ActionResult<IEnumerable<ImageMetadata>>> GetWardrobeImages(Guid wardrobeId)
    {
        var images = await _bll.Images.GetImagesForWardrobeAsync(wardrobeId);
        return Ok(images.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Upload new image
    [Produces("application/json")]
    [Consumes("multipart/form-data")]
    [HttpPost("upload")]
    public async Task<ActionResult<ImageMetadata>> UploadImage(
        IFormFile file, 
        [FromForm] Guid? clothingItemId = null,
        [FromForm] Guid? outfitId = null,
        [FromForm] bool isProfileImage = false)
    {
        if (file.Length == 0) 
            return BadRequest(new Message("No file uploaded"));

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new Message("Invalid file type. Only JPEG, PNG, GIF, and WebP images are allowed."));
        
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            return BadRequest(new Message("File size too large. Maximum size is 5MB."));

        var userId = User.GetUserId();

        try
        {
            await using var stream = file.OpenReadStream();
            var uploadedImage = await _bll.Images.UploadImageAsync(stream, file.FileName, file.ContentType);
            // associate with user
            uploadedImage.UserId = userId;

            // associate with clothing item if specified
            if (clothingItemId.HasValue)
            {
                var clothingItem = await _bll.ClothingItems.FindAsync(clothingItemId.Value, userId);
                if (clothingItem == null)
                    return BadRequest(new Message("Clothing item not found or does not belong to you"));
                
                uploadedImage.ClothingItemId = clothingItemId.Value;
            }
            
            // associate with outfit if specified
            if (outfitId.HasValue)
            {
                var outfit = await _bll.Outfits.FindAsync(outfitId.Value, userId);
                if (outfit == null)
                    return BadRequest(new Message("Outfit not found or does not belong to you"));
                
                uploadedImage.OutfitId = outfitId.Value;
            }

            // Set as profile image if specified
            if (isProfileImage && !clothingItemId.HasValue)
                await _bll.Images.SetProfileImageAsync(userId, uploadedImage.Id);
            
            await _bll.SaveChangesAsync();

            var result = _mapper.Map(uploadedImage)!;
            return CreatedAtAction("GetImage", new { id = uploadedImage.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Update image metadata
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutImage(Guid id, ImageMetadata imageMetadata)
    {
        if (id != imageMetadata.Id)
            return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
      
        var existingImage = await _bll.Images.FindAsync(id, userId);
        if (existingImage == null)
            return NotFound(new Message("Image not found"));
        
        var bllImage = _mapper.Map(imageMetadata);
        if (bllImage == null)
            return BadRequest(new Message("Invalid image data"));
        
        await _bll.Images.UpdateAsync(bllImage, userId);
        await _bll.SaveChangesAsync();

        return NoContent();
    }
    
    
    // Set image as profile image
    [Produces("application/json")]
    [HttpPut("{id:guid}/set-profile")]
    public async Task<IActionResult> SetProfileImage(Guid id)
    {
        var userId = User.GetUserId();

        try
        {
            await _bll.Images.SetProfileImageAsync(userId, id);
            await _bll.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Connect image with clothing item
    [Produces("application/json")]
    [HttpPut("{id}/clothing/{clothingItemId}")]
    public async Task<ActionResult<ImageMetadata>> AssignToClothingItem(Guid id, Guid clothingItemId)
    {
        var userId = User.GetUserId();

        try
        {
            var updatedImage = await _bll.Images.AssignImageToClothingItemAsync(id, clothingItemId, userId);
            await _bll.SaveChangesAsync();
                
            return Ok(_mapper.Map(updatedImage)!);
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
    
    
    // Connect image with outfit
    [Produces("application/json")]
    [HttpPut("{id}/outfit/{outfitId}")]
    public async Task<ActionResult<ImageMetadata>> AssignToOutfit(Guid id, Guid outfitId)
    {
        var userId = User.GetUserId();

        try
        {
            var updatedImage = await _bll.Images.AssignImageToOutfitAsync(id, outfitId, userId);
            await _bll.SaveChangesAsync();
                
            return Ok(_mapper.Map(updatedImage)!);
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
    
    
    // Delete image
    [Produces("application/json")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteImage(Guid id)
    {
        var userId = User.GetUserId();

        try
        {
            var success = await _bll.Images.DeleteImageAsync(id, userId);
            if (!success)
            {
                return NotFound(new Message("Image not found"));
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
}