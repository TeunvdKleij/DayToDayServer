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
    private readonly NoteService _noteService;
    public NoteController(DataContext dataContext, LogService logService, NoteService noteService)
    {
        _dataContext = dataContext;
        _logService = logService;
        _noteService = noteService;
    }
    
    [HttpPost("NoteForADay")]
    public async Task<IActionResult> GetNoteForADay([FromBody] NoteDTO noteDto)
    {
        return await _noteService.GetNoteForADay(noteDto);
    }
    
    [HttpPost("UpdateNote")]
    public async Task<IActionResult> UpdateNote([FromBody] NoteDTO noteDto)
    {
        return await _noteService.UpdateNote(noteDto);
    }
    [HttpPost("RemoveNotesByGroup")]
    public async Task<IActionResult> RemoveNotesByGroup([FromBody] NoteDTO noteDto)
    {
        return await _noteService.RemoveNote(noteDto);
    }
    
}