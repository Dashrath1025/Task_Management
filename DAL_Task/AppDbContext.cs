using DAL_Task.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL_Task
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

         
         public DbSet<AppUser> appUsers { get; set; }
         
         public DbSet<TaskModel> Tasks {  get; set; } 

    }
}
