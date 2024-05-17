using System.ComponentModel.DataAnnotations;

namespace DayToDay.Models;

public class GroupModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<NoteModel> Notes { get; set; }
    public ICollection<TaskModel> Tasks { get; set; }
    
}