using DayToDay.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DayToDay.Data;

public class DataContext : IdentityDbContext<UserModel>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<GroupModel> Group { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<NoteModel> Notes { get; set; }
}