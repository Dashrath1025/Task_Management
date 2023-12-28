using DAL_Task;
using DAL_Task.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL_Task
{
    public interface ITask
    {
        TaskModel GetTaskById(int taskId);
        Task<IdentityUser> GetUserAsync(string userId);
        bool CreateTask(TaskModel task);
        bool UpdateTask(TaskModel task);
        IEnumerable<TaskModel> GetAllTasks();
        Task<bool> Exists(string id);
        Task<bool> UpdateProfileImage(string id,string imageUrl);

         


    }
}
