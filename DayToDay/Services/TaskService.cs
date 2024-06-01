using System.Globalization;
using DayToDay.Controllers;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayToDay.Services;

public class TaskService
{
    private readonly DataContext _dataContext;
    private readonly LogService _logService;
    private readonly ValidationService _validationService;

    public TaskService(DataContext dataContext, LogService logService, ValidationService validationService)
    {
        _dataContext = dataContext;
        _logService = logService;
        _validationService = validationService;
    }

    public async Task<IActionResult> GetTasksForADay(TaskDTO taskDto)
    {
        var oldTasks = _dataContext.Tasks.Where(i => i.dateAdded < DateTime.Now.Date && !i.Done).ToList();
        SetTasksToCurrentDate(oldTasks);

        var groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        DateTime date = DateTime.Now.AddDays((double)taskDto.ChangedDate);

        var taskList = await _dataContext.Tasks.Where(i => i.dateAdded == date.Date && i.GroupId == groupID)
            .ToListAsync();
        return new OkObjectResult(new { tasks = taskList });
    }

    public async void SetTasksToCurrentDate(List<TaskModel> oldTasks)
    {
        if (oldTasks.Count > 0)
        {
            foreach (TaskModel task in oldTasks)
            {
                task.dateAdded = DateTime.Now.Date;
                _dataContext.Tasks.Update(task);
                await _dataContext.SaveChangesAsync();
            }
        }
    }

    public async Task<IActionResult> GetTasksForAGroup(TaskDTO taskDto)
    {
        var groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        var tasks = _dataContext.Tasks.Where(i => i.GroupId == groupID).ToList();
        return new OkObjectResult(new { tasks = tasks });
    }

    public async Task<IActionResult> UpdateTaskDate(TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(UpdateTaskDate), "No task found");
            return new BadRequestObjectResult("No task found");
        }

        return await ProcessTaskDateUpdate(updateTask, task);
    }

    private async Task<IActionResult> ProcessTaskDateUpdate(TaskDTO updateTask, TaskModel task)
    {
        DateTime dateTime;
        if (DateTime.TryParseExact(updateTask.Date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateTime))
        {
            string formattedDateTime = dateTime.ToString("yyyy/MM/dd 00:00:00");
            task.dateAdded = DateTime.Parse(formattedDateTime);
            _dataContext.Update(task);
            await _dataContext.SaveChangesAsync();
            return new OkObjectResult(new { message = task.dateAdded });
        }

        return new BadRequestObjectResult(new { message = "Bad request" });
    }

    public async Task<IActionResult> UpdateTaskStatus(TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(UpdateTaskStatus), "No task found");
            return new BadRequestObjectResult("No task found");
        }

        task.Done = (bool)updateTask.Done;
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new { message = task.Done });
    }

    public async Task<IActionResult> UpdateTaskValue(TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == updateTask.Id.ToString());
        if (task == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(UpdateTaskValue), "No task found");
            return new BadRequestObjectResult("No task found");
        }

        string taskName = _validationService.replaceHTML((string)updateTask.TaskName);
        task.TaskName = taskName;
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new { message = taskName });
    }

    public async Task<IActionResult> AddTask(TaskDTO task)
    {
        if (task.ChangedDate < 0)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(AddTask), "Date selected before current day");
            return new BadRequestObjectResult("Date selected before current day");
        }

        var taskIds = new List<int>();
        int taskId = 0;
        var tasks = _dataContext.Tasks.OrderBy(i => i.TaskId).ToList();
        if (tasks == null)
        {
            _logService.WarningLog(nameof(TaskController), nameof(AddTask), "No task found, new taskId = 0");
            taskId = 0;
        }

        for (int i = 0; i < tasks.Count; i++) taskIds.Add(int.Parse(tasks[i].TaskId));
        for (int i = 0; i < taskIds.Count; i++)
        {
            if (!taskIds.Contains(i))
            {
                taskId = i;
                break;
            }

            if (i + 1 == taskIds.Count) taskId = i + 1;
        }
        if (task.GroupName == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(AddTask), "No groupName provided");
            return new BadRequestObjectResult("No groupName provided");
        }
        if (task.ChangedDate == null)
        { 
            _logService.ErrorLog(nameof(TaskController), nameof(AddTask), "No changedDate provided");
            return new BadRequestObjectResult("No changedDate provided");
        }
        int groupId = _dataContext.Group.Where(i => i.Name == task.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupId == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(AddTask), "No groupId found");
            return new BadRequestObjectResult("No groupId found");
        }

        string taskValue = _validationService.replaceHTML(task.TaskName);
        ProcessAddTask(task, taskValue,taskId,groupId);
        return new OkObjectResult(new { id = taskId });
    }

    public async void ProcessAddTask(TaskDTO task, string taskValue, int taskId, int groupId)
    {
        
        TaskModel newTask = new TaskModel
        {
            TaskId = taskId.ToString(),
            TaskName = taskValue,
            Done = false,
            dateAdded = DateTime.Now.Date.AddDays((double)task.ChangedDate),
            GroupId = groupId
        };
        _dataContext.Tasks.Add(newTask);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<IActionResult> RemoveTask(TaskDTO removeTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == removeTask.Id.ToString());
        _dataContext.Remove(task);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult("Task removed succesfully");
    }

    public async Task<IActionResult> RemoveTasksByGroup(TaskDTO taskDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null)
        {
            _logService.ErrorLog(nameof(TaskController), nameof(RemoveTasksByGroup), "No group found");
            return  new BadRequestObjectResult("No group found");
        }
        var tasks = _dataContext.Tasks.Where(i => i.GroupId == groupID).ToList();
        foreach (var item in tasks) _dataContext.Tasks.Remove(item);
        await _dataContext.SaveChangesAsync();
        return new OkObjectResult(new { status = 200, message = "Removed all from group " + taskDto.GroupName });
    }
}