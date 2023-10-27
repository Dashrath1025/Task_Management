using DAL_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(AppDbContext db, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
 

        [HttpGet("getrole")]
        public IActionResult GetRole()
        {
            var roles = _db.Roles.ToList();
            return Ok(roles);
        } 


        [HttpPost("addrole")]

        public async Task<IActionResult> AddRole(IdentityRole roleobj)
        {
            if(await _roleManager.RoleExistsAsync(roleobj.Name))
            {
                return Ok("Role already exist!");
            }

            await _roleManager.CreateAsync(new IdentityRole { Name = roleobj.Name });
            return Ok("Role created success");
        }



        [HttpPut("updaterole")]

        public async Task<IActionResult> UpdateRole(IdentityRole roleObj)
        {
            var objFromDb= await _db.Roles.FirstOrDefaultAsync(u=>u.Id == roleObj.Id);

            if(objFromDb==null)
            {
                return NotFound("Role Not found");
            }

            objFromDb.Name= roleObj.Name;
            objFromDb.NormalizedName= roleObj.Name.ToUpper();


            var result= await _roleManager.UpdateAsync(objFromDb);
            return Ok("Update success");
        }


        [HttpDelete("deleterole")]
        public async Task<IActionResult> Delete(string id)
        
        {
            var objfromDb= await _db.Roles.FirstOrDefaultAsync(t=>t.Id==id);

            if (objfromDb == null)
            {
                return NotFound("Role not found");
            }

            var userRoleAssign = _db.UserRoles.Where(w => w.RoleId == id).Count();

            if (userRoleAssign > 0)
            {
                return BadRequest("can not delete role, this user is assign a role");
            }

            await _roleManager.DeleteAsync(objfromDb);
            return Ok("Role Deleted success");
        }

        [HttpGet("getid/{id:guid}")]

        public async Task<IActionResult> GetRole([FromRoute] string id)
        {
            var obj= await _db.Roles.FirstOrDefaultAsync(w=>w.Id==id);

            if (obj==null)
            {
                return NotFound();
            }

            return Ok(obj);
        }

    }
}
