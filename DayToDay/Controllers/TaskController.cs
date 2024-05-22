using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private DataContext _dataContext;
    public TaskController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    [HttpPost("TasksForADay")]
    public async Task<IActionResult> GetTasksForADay([FromBody] TaskDTO taskDto)
    {
        var OldTasks = _dataContext.Tasks.Where(i => i.dateAdded < DateTime.Now.Date && !i.Done).ToList();
        if (OldTasks.Count > 0)
        {
            foreach (TaskModel task in OldTasks)
            {
                task.dateAdded = DateTime.Now.Date;
                _dataContext.Tasks.Update(task);
                await _dataContext.SaveChangesAsync();
            }
        }
        var groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        DateTime date = DateTime.Now.AddDays((double)taskDto.ChangedDate);
        var tasks = await _dataContext.Tasks.Where(i => i.dateAdded == date.Date && i.GroupId == groupID).ToListAsync();
        return Ok(new { tasks = tasks });
    }

    [HttpPost("TasksForAGroup")]
    public async Task<IActionResult> GetTasksForAGroup([FromBody] TaskDTO taskDto)
    {
        var groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        var tasks = _dataContext.Tasks.Where(i => i.GroupId == groupID).ToList();
        return Ok(new {tasks = tasks});
    }
    
    [HttpPut("UpdateTaskDate")]
    public async Task<IActionResult> UpdateTaskDate([FromBody] TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null) return BadRequest();
        DateTime dateTime;
        if (DateTime.TryParseExact(updateTask.Date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            string formattedDateTime = dateTime.ToString("yyyy/MM/dd 00:00:00");
            task.dateAdded = DateTime.Parse(formattedDateTime);
            _dataContext.Update(task);
            await _dataContext.SaveChangesAsync();
            return Ok(new {message=task.dateAdded});
          
        }
        return BadRequest(new { message = "Bad request" });
       
        
    }
    [HttpPut("UpdateTaskStatus")]
    public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null) return BadRequest();
        task.Done = (bool)updateTask.Done;
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return Ok(new {message=task.Done});
    }
    
    [HttpPut("UpdateTaskValue")]
    public async Task<IActionResult> UpdateTaskValue([FromBody] TaskDTO updateTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == (updateTask.Id).ToString());
        if (task == null) return BadRequest();
        task.TaskName = (string)updateTask.TaskName;
        _dataContext.Update(task);
        await _dataContext.SaveChangesAsync();
        return Ok(new {message=task.TaskName});
    }

    [HttpPost("AddTask")]
    public async Task<IActionResult> AddTask([FromBody] TaskDTO task)
    {
        if (task.ChangedDate < 0) return BadRequest();
        var taskIds = new List<int>();
        int taskId = 0;
        var tasks = _dataContext.Tasks.OrderBy(i => i.TaskId).ToList();
        if (tasks == null) taskId = 0;
        for (int i = 0; i < tasks.Count; i++)
        {
            taskIds.Add(int.Parse(tasks[i].TaskId));
        }
        for (int i = 0; i < taskIds.Count; i++)
        {
            if(taskIds.Contains(i)) Console.WriteLine(i);
            else if (!taskIds.Contains(i))
            { 
                taskId = i; 
                break;
            }
            if (i + 1 == taskIds.Count) taskId = i + 1;
        }
        if (task.GroupName == null) return BadRequest();
        if (task.ChangedDate == null) return BadRequest();
        int groupId = _dataContext.Group.Where(i => i.Name == task.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupId == null) return BadRequest();
        TaskModel newTask = new TaskModel
        {
            TaskId = taskId.ToString(),
            TaskName = task.TaskName,
            Done = false,
            dateAdded = DateTime.Now.Date.AddDays((double)task.ChangedDate),
            GroupId = groupId
        };
        _dataContext.Tasks.Add(newTask);
        await _dataContext.SaveChangesAsync();
        return Ok(new {id=taskId});
    }
    

    [HttpPost("DeleteTask")]
    public async Task<IActionResult> DeleteTask([FromBody] TaskDTO deleteTask)
    {
        TaskModel task = _dataContext.Tasks.FirstOrDefault(i => i.TaskId == deleteTask.Id.ToString());
        _dataContext.Remove(task);
        await _dataContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("RemoveTasksByGroup")]
    public async Task<IActionResult> RemoveNotesByGroup([FromBody] TaskDTO taskDto)
    {
        int groupID = _dataContext.Group.Where(i => i.Name == taskDto.GroupName).Select(i => i.Id).FirstOrDefault();
        if (groupID == null) return BadRequest();
        var tasks = _dataContext.Tasks.Where(i => i.GroupId == groupID).ToList();
        foreach (var item in tasks) _dataContext.Tasks.Remove(item);
        await _dataContext.SaveChangesAsync();
        return Ok(new { status = 200, message = "Deleted all from group " + taskDto.GroupName });
    }
}