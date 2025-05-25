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
public class PlannerEntryController : ControllerBase
{
    private readonly ILogger<PlannerEntryController> _logger;
    private readonly IAppBll _bll;
    private readonly PlannerEntryMapper _mapper = new PlannerEntryMapper();

    public PlannerEntryController(IAppBll bll, ILogger<PlannerEntryController> logger)
    {
        _logger = logger;
        _bll = bll;
    }
    
    
    // Get all planner entries
    [Produces("application/json")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlannerEntry>>> GetPlannerEntries()
    {
        var userId = User.GetUserId();
        var entries = await _bll.PlannerEntries.GetUserPlannerEntriesAsync(userId);
        return Ok(entries.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get planner entry by id
    [Produces("application/json")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlannerEntry>> GetPlannerEntry(Guid id)
    {
        var userId = User.GetUserId();
        var entry = await _bll.PlannerEntries.FindAsync(id, userId);

        if (entry == null) return NotFound(new Message("Planner entry not found"));

        return _mapper.Map(entry)!;
    }
    
    
    // Get entries for a specific date range
    [Produces("application/json")]
    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<PlannerEntry>>> GetPlannerEntriesForDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var userId = User.GetUserId();
            
        try
        {
            var entries = await _bll.PlannerEntries.GetUserEntriesForDateRangeAsync(userId, startDate, endDate);
            return Ok(entries.Select(x => _mapper.Map(x)!).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Get entries for a specific month
    [Produces("application/json")]
    [HttpGet("month/{year}/{month}")]
    public async Task<ActionResult<IEnumerable<PlannerEntry>>> GetPlannerEntriesForMonth(int year, int month)
    {
        var userId = User.GetUserId();
            
        try
        {
            var entries = await _bll.PlannerEntries.GetUserEntriesForMonthAsync(userId, year, month);
            return Ok(entries.Select(x => _mapper.Map(x)!).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Get entry for a date
    [Produces("application/json")]
    [HttpGet("date/{date}")]
    public async Task<ActionResult<PlannerEntry>> GetPlannerEntryForDate(DateTime date)
    {
        var userId = User.GetUserId();
        var entry = await _bll.PlannerEntries.GetUserEntryForDateAsync(userId, date);

        if (entry == null) return NotFound(new Message("No planner entry found for this date"));

        return _mapper.Map(entry)!;
    }
    
    
    // Get today's entries
    [Produces("application/json")]
    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<PlannerEntry>>> GetTodayEntries()
    {
        var userId = User.GetUserId();
        var entries = await _bll.PlannerEntries.GetTodayEntriesAsync(userId);
        return Ok(entries.Select(x => _mapper.Map(x)!).ToList());
    }
    
    // Get upcoming entries
    [Produces("application/json")]
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<PlannerEntry>>> GetUpcomingEntries([FromQuery] int maxResults = 10)
    {
        var userId = User.GetUserId();
        var entries = await _bll.PlannerEntries.GetUpcomingEntriesAsync(userId, maxResults);
        return Ok(entries.Select(x => _mapper.Map(x)!).ToList());
    }
    
    
    // Get planned outfit for this date
    [Produces("application/json")]
    [HttpGet("outfit/{date}")]
    public async Task<ActionResult<Outfit>> GetPlannedOutfitForDate(DateTime date)
    {
        var userId = User.GetUserId();
        var outfit = await _bll.PlannerEntries.GetPlannedOutfitForDateAsync(userId, date);

        if (outfit == null) return NotFound(new Message("No outfit planned for this date"));

        var outfitMapper = new OutfitMapper();
        return outfitMapper.Map(outfit)!;
    }
    
    
    // Check if the outfit is planned
    [Produces("application/json")]
    [HttpGet("check/{outfitId}/{date}")]
    public async Task<ActionResult<object>> IsOutfitPlannedForDate(Guid outfitId, DateTime date)
    {
        var isPlanned = await _bll.PlannerEntries.IsOutfitPlannedForDateAsync(outfitId, date);
            
        return Ok(new
        {
            OutfitId = outfitId,
            Date = date.ToString("yyyy-MM-dd"),
            IsPlanned = isPlanned
        });
    }
    
    
    // check if user has any outift planned
    [Produces("application/json")]
    [HttpGet("has-outfit/{date}")]
    public async Task<ActionResult<object>> HasPlannedOutfitForDate(DateTime date)
    {
        var userId = User.GetUserId();
        var hasOutfit = await _bll.PlannerEntries.HasPlannedOutfitForDateAsync(userId, date);
            
        return Ok(new
        {
            Date = date.ToString("yyyy-MM-dd"),
            HasPlannedOutfit = hasOutfit,
            Message = hasOutfit ? "Outfit is planned for this date" : "No outfit planned for this date"
        });
    }
    
    
    // Create new planner entry
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPost]
    public async Task<ActionResult<PlannerEntry>> PostPlannerEntry(PlannerEntryCreate plannerEntryCreate)
    {
        var userId = User.GetUserId();
        plannerEntryCreate.UserId = userId;
            
        try
        {
            var bllPlannerEntry = _mapper.Map(plannerEntryCreate);
            var createdEntry = await _bll.PlannerEntries.CreatePlannerEntryAsync(bllPlannerEntry);
            await _bll.SaveChangesAsync();

            var result = _mapper.Map(createdEntry)!;
            return CreatedAtAction("GetPlannerEntry", new { id = createdEntry.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Update planner entry
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlannerEntry(Guid id, PlannerEntry plannerEntry)
    {
        if (id != plannerEntry.Id) return BadRequest(new Message("ID mismatch"));
        
        var userId = User.GetUserId();
        if (plannerEntry.UserId != userId)
            return Forbid("Cannot modify other user's planner entry");
        
        try
        {
            var bllPlannerEntry = _mapper.Map(plannerEntry);
            if (bllPlannerEntry == null)
                return BadRequest(new Message("Invalid planner entry data"));
            
            var updatedEntry = await _bll.PlannerEntries.UpdatePlannerEntryAsync(bllPlannerEntry);
            if (updatedEntry == null)
                return NotFound(new Message("Planner entry not found"));
            
            await _bll.SaveChangesAsync();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new Message(ex.Message));
        }
    }
    
    
    // Delete planner entry
    [Produces("application/json")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlannerEntry(Guid id)
    {
        var userId = User.GetUserId();
            
        var success = await _bll.PlannerEntries.DeletePlannerEntryAsync(id, userId);
        if (!success) return NotFound(new Message("Planner entry not found"));

        await _bll.SaveChangesAsync();
        return NoContent();
    }
}