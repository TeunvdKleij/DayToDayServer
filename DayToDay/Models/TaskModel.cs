using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DayToDay.Models;

// public enum RepetativeTypes{
//     Day,
//     Week,
//     Month,
//     TwoMonths,
//     ThreeMonths,
//     HalfYear,
//     Year
// }
public class TaskModel
{
    [Key]
    public string TaskId { get; set; }
    public string TaskName { get; set; }
    public bool Done { get; set; }
    public DateTime dateAdded { get; set; }
    
    public int GroupId { get; set; }
    
    [ForeignKey("GroupId")]
    public GroupModel Group { get; set; }

}