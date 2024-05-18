using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private DataContext _dataContext;

    public GroupController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet("GetGroups")]
    public async Task<IActionResult> GetGroups()
    {
        var res = _dataContext.Group.Select(i => i.Name).ToList();
        if (res == null) return BadRequest(new {showAddGroup = true});
        return Ok(new { groups = res });
    }
    [HttpPost("AddGroup")]
    public async Task<IActionResult> AddGroup([FromBody] GroupDTO groupDto)
    {
        if (groupDto.Name == null) return BadRequest();
        GroupModel newGroup = new GroupModel
        {
            Name = groupDto.Name
        };
        _dataContext.Group.Add(newGroup);
        await _dataContext.SaveChangesAsync();
        
        var res = _dataContext.Group.Select(i => i.Name).ToList();
        if (res == null) return BadRequest();
        return Ok(new { groups = res });
    }
    [HttpPost("RemoveGroup")]
    public async Task<IActionResult> DeleteGroup([FromBody] GroupDTO groupDto)
    {
        GroupModel group = _dataContext.Group.Where(i => i.Name == groupDto.Name).FirstOrDefault();
        _dataContext.Group.Remove(group);
        await _dataContext.SaveChangesAsync();
        return Ok(new { status = 200, message = "Deleted all from group " + groupDto.Name });
    }
    
}