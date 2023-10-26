using DAL_Task;
using DAL_Task.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BLL_Task
{
    public class TaskRepository : ITask
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TaskRepository(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public bool CreateTask(TaskModel task)
        {
            _context.Tasks.Add(task);   
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> Exists(string id)
        {
            return await _context.Users.AnyAsync(x=>x.Id == id);
        }

        public IEnumerable<TaskModel> GetAllTasks()
        {
            return _context.Tasks.ToList(); 
        }

        public TaskModel GetTaskById(int taskId)
        {
            return _context.Tasks.First(d => d.TaskId == taskId);
        }

        public async Task<IdentityUser> GetUserAsync(string userId)
        {
            return await _context.Users.FirstAsync(x => x.Id == userId);
        }

        public async Task<bool> UpdateProfileImage(string id, string imageUrl)
        {
           var user= await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                user.Image = imageUrl;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public bool UpdateTask(TaskModel task)
        {
            _context.ChangeTracker.Clear();
            _context.Tasks.Update(task);
            _context.SaveChanges();
            return true;

        }


    }
}
