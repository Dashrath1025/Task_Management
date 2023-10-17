﻿using BLL_Task;
using DAL_Task;
using DAL_Task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks.Sources;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITask task;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public TasksController(ITask task, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.task = task;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        [HttpGet("getalltask")]
        public IEnumerable<TaskModel> GetAll()
         {
            return  task.GetAllTasks();
        }

        [HttpPost("addtask")]

        public async Task<IActionResult> AddTask([FromBody] TaskModel model)
        {

            var user = await userManager.FindByIdAsync(model.AssigneeId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            model.AssigneeId = user.Id;

            var tesk =  task.CreateTask(model);

            

            return Ok("Task created success");
        }

        [HttpPut("update")]

        public IActionResult UpdateTask(TaskModel model)
        {
            var id = task.GetTaskById(model.TaskId);

            if (id == null)
            {
                return NotFound();
            }

            var update= task.UpdateTask(model);
            return Ok(update);
        }

        [HttpGet("filtertask")]

        public IEnumerable<TaskModel> GetAll([FromQuery] DateTime? dueDate, string? status,string? assignee)
        {
            var alltasks= task.GetAllTasks();

            var filteredTask = alltasks;

            if (dueDate.HasValue)
            {
                filteredTask= filteredTask.Where(t=>t.DueDate.Date==dueDate.Value.Date);
            }

            if(!string.IsNullOrWhiteSpace(status))
            {
                filteredTask=filteredTask.Where(e=>e.Status.ToLower()==status.ToLower());
            }

            if(!string.IsNullOrWhiteSpace(assignee))
            {
                filteredTask=filteredTask.Where(r=>r.AssigneeId==assignee);
            }

            return filteredTask.ToList();   

        }

        
    }
}