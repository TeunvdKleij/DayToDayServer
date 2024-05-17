using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayToDay.Models;

public class NoteModel
{
    [Key]
    public int NoteId { get; set; }
    public string NoteText { get; set; }
    public DateTime dateAdded { get; set; }
    
    public int GroupId { get; set; }
    
    [ForeignKey("GroupId")]
    public GroupModel Group { get; set; }
    
    
}