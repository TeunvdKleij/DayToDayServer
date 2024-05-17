using System.Globalization;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayToDay.Services;

public interface ITaskService
{
    Task<List<TaskModel>> GetTasksForDayAsync(TaskDTO taskDto);
    Task<IActionResult> UpdateTaskDate(TaskDTO updateTask);
    Task<IActionResult> UpdateTaskStatus(TaskDTO updateTask);
}

public class TaskService : ITaskService
{
    private readonly DataContext _dataContext;

    public TaskService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<TaskModel>> GetTasksForDayAsync(TaskDTO taskDto)
    {
        var oldTasks = _dataContext.Tasks.Where(i => i.dateAdded < DateTime.Now.Date && !i.Done).ToList();
        if (oldTasks.Count > 0)
        {
            foreach (var task in oldTasks)
            {
                task.dateAdded = DateTime.Now.Date;
                _dataContext.Tasks.Update(task);
            } await _dataContext.SaveChangesAsync();
        }
        var groupId = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        var date = DateTime.Now.AddDays((double)taskDto.ChangedDate);
        var tasks = await _dataContext.Tasks.Where(i => i.dateAdded == date.Date && i.GroupId == groupId).ToListAsync();
        return tasks;
    }
    public async Task<IActionResult> UpdateTaskDate(TaskDTO updateTask)
    {
        var task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == updateTask.Id.ToString());
        if (task == null) return new BadRequestResult();
        DateTime dateTime;
        if (!DateTime.TryParseExact(updateTask.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) return new BadRequestObjectResult(new { message = "Bad request" });
        string formattedDateTime = dateTime.ToString("yyyy/MM/dd 00:00:00");
        task.dateAdded = DateTime.Parse(formattedDateTime);
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new { message = task.dateAdded });
    }

    public async Task<IActionResult> UpdateTaskStatus(TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null) return new BadRequestResult();
        task.Done = (bool)updateTask.Done;
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new {message=task.Done});
    }
}