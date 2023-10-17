
//using DAL_Task;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;


//namespace Task_Management.Controllers
//{
//    public class UserController : Controller
//    {
//        private readonly AppDbContext _db;
//        private readonly UserManager<AppUser> _userManager;

//        public UserController(UserManager<AppUser> userManager, AppDbContext db)
//        {
//            _db = db;
//            _userManager = userManager;
//        }

//        [HttpGet("getroles")]
//        public IActionResult Get()
//        {
//            var userList = _db.appUsers.ToList();
//            var userRole = _db.UserRoles.ToList();
//            var roles = _db.Roles.ToList();
//            foreach (var user in userList)
//            {
//                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
//                if (role == null)
//                {
//                    user.Role = "None";
//                }
//                else
//                {
//                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
//                }
//            }

//            return Ok(userList);

//        }

//        [HttpGet("{userId}")]
//        public IActionResult Edit(string userId)
//        {
//            var objFromDb = _db.appUsers.FirstOrDefault(u => u.Id == userId);
//            if (objFromDb == null)
//            {
//                return NotFound();
//            }
//            var userRole = _db.UserRoles.ToList();
//            var roles = _db.Roles.ToList();
//            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
//            if (role != null)
//            {
//                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
//            }
//            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
//            {
//                Text = u.Name,
//                Value = u.Id
//            });
//            return View(objFromDb);
//        }


//        [HttpPut("updaterole")]
       
//        public async Task<IActionResult> Update(AppUser user)
//        {
//            if (ModelState.IsValid)
//            {
//                var objFromDb = _db.appUsers.FirstOrDefault(u => u.Id == user.Id);
//                if (objFromDb == null)
//                {
//                    return NotFound();
//                }

//                var userRole = _db.UserRoles.FirstOrDefault(e => e.UserId == objFromDb.Id);
//                if (userRole != null)
//                {
//                    var previuousRole = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();

//                    await _userManager.RemoveFromRoleAsync(objFromDb, previuousRole);

//                }
//                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
//                _db.SaveChanges();
//                return Ok("User role assign success");
//            }

//            user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
//            {
//                Text = u.Name,
//                Value = u.Id
//            });
//            return Ok(user);
//        }


//        [HttpPost]

//        public IActionResult LockUnlcok(string userId)
//        {
//            var objFromDb = _db.ApplicationUser.FirstOrDefault(e => e.Id == userId);
//            if (objFromDb == null)
//            {
//                return NotFound();
//            }
//            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
//            {
//                objFromDb.LockoutEnd = DateTime.Now;

//            }
//        }
//    }
//}