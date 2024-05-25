using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using DayToDay.Data;
using DayToDay.Models;
using DayToDay.Models.DTO;
using DayToDay.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DayToDay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private DataContext _dataContext;
    private readonly TaskService _taskService;
    public TaskController(DataContext dataContext, TaskService taskService)
    {
        _dataContext = dataContext;
        _taskService = taskService;
    }
    
    [HttpPost("TasksForADay")]
    public async Task<IActionResult> GetTasksForADay([FromBody] TaskDTO taskDto)
    {
        return await _taskService.GetTasksForADay(taskDto);
    }

    [HttpPost("TasksForAGroup")]
    public async Task<IActionResult> GetTasksForAGroup([FromBody] TaskDTO taskDto)
    {
        return await _taskService.GetTasksForAGroup(taskDto);
    }
    
    [HttpPut("UpdateTaskDate")]
    public async Task<IActionResult> UpdateTaskDate([FromBody] TaskDTO updateTask)
    {
        return await _taskService.UpdateTaskDate(updateTask);
    }
    [HttpPut("UpdateTaskStatus")]
    public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskDTO updateTask)
    {
        return await _taskService.UpdateTaskStatus(updateTask);
    }
    
    [HttpPut("UpdateTaskValue")]
    public async Task<IActionResult> UpdateTaskValue([FromBody] TaskDTO updateTask)
    {
        return await _taskService.UpdateTaskValue(updateTask);
    }

    [HttpPost("AddTask")]
    public async Task<IActionResult> AddTask([FromBody] TaskDTO task)
    {
        return await _taskService.AddTask(task);
    }
    
    [HttpPost("RemoveTask")]
    public async Task<IActionResult> RemoveTask([FromBody] TaskDTO removeTask)
    {
        return await _taskService.RemoveTask(removeTask);
    }

    [HttpPost("RemoveTasksByGroup")]
    public async Task<IActionResult> RemoveTasksByGroup([FromBody] TaskDTO taskDto)
    {
        return await _taskService.RemoveTasksByGroup(taskDto);
    }
}