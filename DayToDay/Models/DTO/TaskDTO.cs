namespace DayToDay.Models.DTO;

public class TaskDTO
{
    public int? Id { get; set; }
    public bool? Done { get; set; }
    public string? TaskName { get; set; }
    public int? ChangedDate { get; set; }
    public string? Date { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
}