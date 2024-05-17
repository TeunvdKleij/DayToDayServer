using DayToDay.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DayToDay.Models.DTO;

public class NoteDTO
{
    public int? Id { get; set; }
    public string? NoteText { get; set; }
    public int? ChangedDate { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    
    public async Task<NoteModel> AddNote(DataContext _dataContext, NoteDTO noteDto)
    {
        try
        {
            if (string.IsNullOrEmpty(noteDto.NoteText)) return null;
            int groupID = _dataContext.Group.Where(i => i.Name == noteDto.GroupName).Select(i => i.Id).FirstOrDefault();
            if (groupID == null) return null;
            NoteModel newNote = new NoteModel
            {
                NoteText = noteDto.NoteText,
                dateAdded = DateTime.Now.Date.AddDays((double)noteDto.ChangedDate),
                GroupId = groupID
            };
            _dataContext.Notes.Add(newNote);
            await _dataContext.SaveChangesAsync();
            return newNote;
        }
        catch
        {
            return null;
        }
    }
}