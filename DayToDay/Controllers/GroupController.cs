using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using DayToDay.Services;
using Microsoft.AspNetCore.Mvc;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly GroupService _groupService;

    public GroupController(GroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet("GetGroups")]
    public async Task<IActionResult> GetGroups()
    {
        return await _groupService.GetGroups();
    }
    [HttpPost("AddGroup")]
    public async Task<IActionResult> AddGroup([FromBody] GroupDTO groupDto)
    {
        return await _groupService.GetGroupsAfterAddingGroup(groupDto);
    }
    [HttpPost("RemoveGroup")]
    public async Task<IActionResult> RemoveGroup([FromBody] GroupDTO groupDto)
    {
        return await _groupService.RemoveGroup(groupDto);
    }
    
    
}