using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using DayToDay.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private DataContext _dataContext;
    private readonly LogService _logService;
    public NoteController(DataContext dataContext, LogService logService)
    {
        _dataContext = dataContext;
        _logService = logService;
    }
    
    [HttpPost("NoteForADay")]
    public async Task<IActionResult> GetNoteForADay([FromBody] NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.WarningLog(nameof(NoteController), nameof(RemoveNotesByGroup), "No group found");
            return Ok(new {status = 204, groupID = 9999999});
        }
        DateTime date = DateTime.Now.Date.AddDays((double)noteDto.ChangedDate);
        var notes = _dataContext.Notes.ToList();
        foreach (var item in notes)
        {
            if (item.dateAdded == date && item.GroupId == groupID)
            {
                return Ok(new { status = 200, note = item,  name = noteDto.GroupName});
            }
        }
        return Ok(new {status = 204});
    }
    
    [HttpPost("UpdateNote")]
    public async Task<IActionResult> UpdateNote([FromBody] NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.ErrorLog(nameof(NoteController), nameof(RemoveNotesByGroup), "No group found");
            return BadRequest();
        }
        DateTime date = DateTime.Now.Date.AddDays((double)noteDto.ChangedDate);
        var notes = _dataContext.Notes.ToList();
        foreach (var item in notes)
        {
            if (item.dateAdded == date && item.GroupId == groupID)
            {
                if (string.IsNullOrEmpty(noteDto.NoteText))
                {
                    _dataContext.Notes.Remove(item);
                    await _dataContext.SaveChangesAsync();
                    _logService.WarningLog(nameof(NoteController), nameof(UpdateNote), "Note empty");
                    return Ok(new { status = 204});
                }
                item.NoteText = noteDto.NoteText;
                _dataContext.Notes.Update(item);
                await _dataContext.SaveChangesAsync();
                return Ok(new { status = 200, note = item });
                
            }
        }
        NoteModel newNote = await noteDto.AddNote(_dataContext, noteDto);
        return Ok(new {note = newNote, noteText = noteDto.NoteText});
    }
    [HttpPost("RemoveNotesByGroup")]
    public async Task<IActionResult> RemoveNotesByGroup([FromBody] NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.ErrorLog(nameof(NoteController), nameof(RemoveNotesByGroup), "No group found");
            return BadRequest("No group found");
        }
        var notes = _dataContext.Notes.Where(i => i.GroupId == groupID).ToList();
        if (notes.Count == 0) _logService.WarningLog(nameof(NoteController), nameof(RemoveNotesByGroup), "No notes found");
        
        foreach (var item in notes) _dataContext.Notes.Remove(item);
        await _dataContext.SaveChangesAsync();
        
        _logService.InformationLog(nameof(NoteController), nameof(RemoveNotesByGroup), "Deleted all notes from group");
        return Ok(new { status = 200, message = "Deleted all from group " + noteDto.GroupName });
    }
    
}