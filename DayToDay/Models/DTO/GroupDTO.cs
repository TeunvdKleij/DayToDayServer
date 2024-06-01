using System.ComponentModel.DataAnnotations;

namespace DayToDay.Models.DTO;

public class GroupDTO
{
    [MaxLength(20)]
    public string? Name { get; set; }
}