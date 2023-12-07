
using DAL_Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, AppDbContext db,RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("getroles")]
        public IActionResult Get()
        {
            var userList = _db.appUsers.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }

            return Ok(userList);

        }

        //[HttpGet("getrole")]
        //public async Task<IActionResult> Edit(AppUser user)
        //{
        //    var objFromDb = await _db.appUsers.FirstOrDefaultAsync(u => u.Id == userId);
        //    if (objFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    var userRole = await _db.UserRoles.ToListAsync();
        //    var roles = await _db.Roles.ToListAsync();
        //    var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
        //    if (role != null)
        //    {
        //        objFromDb.RoleId = roles.First(u => u.Id == role.RoleId).Id;
        //    }
        //    objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        //    {
        //        Text = u.Name,
        //        Value = u.Id
        //    });
        //    return View(objFromDb);

        //    var userrole = await _db.UserRoles.ToListAsync();


        //    var roles= await _db.Roles.ToListAsync();

        //    var role=userrole.FirstOrDefault(each=>each.UserId == user.Id);

        //    var userroles = user.Role = roles.FirstOrDefault(r => r.Id == role.RoleId).Name;


        //    var allroles= _roleManager.Roles.Select(r => r.Name).ToList();


        //    var response = new UserResponse
        //    {
        //        UserId = user.Id,
        //        Email = user.Email,
        //        RoleId = role.RoleId,
        //        Name = user.UserName,
        //        Rolelist = allroles
        //    };

        //    return Ok(response);
        //}


        [HttpPost("updaterole")]
        public async Task<IActionResult> AssignRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var existingRole = await _roleManager.FindByNameAsync(roleName);

            if (existingRole == null)
            {
                return BadRequest("Role not found");
            }

            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new Response{ Status="Success",Message="User Assign Successfully"});
            }
            else
            {
                return BadRequest("Failed to assign role");
            }
        }

        [HttpGet("getrolename/{userId:guid}")]
        public async Task<IActionResult> GetRoleNameByUserId([FromRoute] string userId)
        {
            try
            {
                AppUser user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Any())
                {
                    var roleName = await _roleManager.FindByNameAsync(roles[0]);

                    if (roleName != null)
                    {
                        var role = roleName.Name;
                        return Ok(role.ToString());
                    }
                }

                return NotFound("Role not found for the user");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // You might want to log the exception details using a logging library
                Console.Error.WriteLine($"Error in GetRoleNameByUserId: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }



        [HttpPost("lockunlock")]
        public IActionResult LockUnlock(string userId)
        {
            var objFromDb = _db.appUsers.FirstOrDefault(e => e.Id == userId);

            if (objFromDb == null)
            {
                return NotFound();
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
                _db.SaveChanges();
                return Ok(new { message = "unlocked" });
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddMonths(1);
                _db.SaveChanges();
                return Ok(new { message = "locked" });
            }
        }



    }
}