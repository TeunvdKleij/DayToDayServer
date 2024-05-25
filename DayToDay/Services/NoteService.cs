using DayToDay.Controllers;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DayToDay.Services;

public class NoteService
{
    private readonly DataContext _dataContext;
    private readonly LogService _logService;

    public NoteService(DataContext dataContext, LogService logService)
    {
        _dataContext = dataContext;
        _logService = logService;
    }

    public async Task<OkObjectResult> GetNoteForADay(NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.WarningLog(nameof(NoteController), nameof(GetNoteForADay), "No group found");
            return new OkObjectResult(new {status = 204, groupID = 9999999});
        }
        DateTime date = DateTime.Now.Date.AddDays((double)noteDto.ChangedDate);
        var notes = _dataContext.Notes.ToList();
        foreach (var item in notes)
        {
            if (item.dateAdded == date && item.GroupId == groupID)
            {
                return new OkObjectResult(new { status = 200, note = item,  name = noteDto.GroupName});
            }
        }
        return new OkObjectResult(new {status = 204});
    }

    public async Task<IActionResult> UpdateNote(NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.ErrorLog(nameof(NoteController), nameof(UpdateNote), "No group found");
            return new BadRequestObjectResult("No group found");
        }
        DateTime date = DateTime.Now.Date.AddDays((double)noteDto.ChangedDate);
        
        var notes = _dataContext.Notes.ToList();
        foreach (var item in notes)
        {
            if (item.dateAdded == date && item.GroupId == groupID)
            {
                if (string.IsNullOrEmpty(noteDto.NoteText)) return await ProcessRemoveNote(item);
                return await ProcessUpdateNote(item, noteDto);
            }
        }
        NoteModel newNote = await noteDto.AddNote(_dataContext);
        return new OkObjectResult(new {note = newNote, noteText = noteDto.NoteText});
    }
    
    private async Task<IActionResult> ProcessUpdateNote(NoteModel item, NoteDTO noteDto)
    {
        item.NoteText = noteDto.NoteText;
        _dataContext.Notes.Update(item);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new { status = 200, note = item });
    }
    
    private async Task<IActionResult> ProcessRemoveNote(NoteModel item)
    {
        _dataContext.Notes.Remove(item);
        await _dataContext.SaveChangesAsync();
        _logService.WarningLog(nameof(NoteController), nameof(UpdateNote), "Note empty");
        return new OkObjectResult(new { status = 204});
    }

    public async Task<IActionResult> RemoveNote(NoteDTO noteDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.ErrorLog(nameof(NoteController), nameof(RemoveNote)+"ByGroup", "No group found");
            return new BadRequestObjectResult("No group found");
        }
        var notes = _dataContext.Notes.Where(i => i.GroupId == groupID).ToList();
        if (notes.Count == 0) _logService.WarningLog(nameof(NoteController), nameof(RemoveNote)+"ByGroup", "No notes found");
        
        foreach (var item in notes) _dataContext.Notes.Remove(item);
        await _dataContext.SaveChangesAsync();
        
        _logService.InformationLog(nameof(NoteController), nameof(RemoveNote)+"ByGroup", "Removed all notes from group");
        return new OkObjectResult(new { status = 200, message = "Removed all from group " + noteDto.GroupName });
    }
    

}