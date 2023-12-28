using BLL_Task;
using DAL_Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITask task;
        private readonly IImageRepository imageRepository;


        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ITask task, IImageRepository imageRepository, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            this.task = task;
            this.imageRepository = imageRepository;
            _signInManager = signInManager;
        }


        [HttpPost]
        [Route("register")]

        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (!await _roleManager.RoleExistsAsync(WC.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(WC.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(WC.UserRole));
            }


            var userExist = await _userManager.FindByNameAsync(model.FirstName);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Already Exist!" });

            }


            AppUser user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.FirstName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                DOB = model.DOB,
                City = model.City,
                Gender = model.Gender,
                Mobile = model.Mobile,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed ! please check user details and try again later" });
            }


            //if (model.RoleSelected != null && model.RoleSelected.Length > 0 && model.RoleSelected == "Admin")
            //{
            //    await _userManager.AddToRoleAsync(user, WC.AdminRole);
            //}
            //else
            //{
            //    await _userManager.AddToRoleAsync(user, WC.UserRole);
            //}

            int count = _userManager.Users.Count();

            if (count == 1)
            {
                await _userManager.AddToRoleAsync(user, WC.AdminRole);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, WC.UserRole);

            }

            //List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem()
            //{
            //    Value = WC.AdminRole,
            //    Text = WC.AdminRole
            //});
            //list.Add(new SelectListItem()
            //{
            //    Value = WC.AdminRole,
            //    Text = WC.UserRole
            //});
            //model.RoleList= list;

            return Ok(new Response { Status = "Success", Message = "User Created Successfully!" });

        }

        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, lockoutOnFailure: true, isPersistent: false);

                if (signInResult.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
            {
                new Claim("name",user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim("role", userRole));
                        authClaims.Add(new Claim("id", user.Id));
                    }

                    var token = GenerateJwtToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                else if (signInResult.IsLockedOut)
                {
                    return StatusCode(423, "Locked"); // Return a specific status code indicating that the user is locked out
                }
                else if (signInResult.IsNotAllowed)
                {
                    return StatusCode(403, "User is not allowed to sign in. Please contact the system administrator.");
                }
                else
                {
                    return Unauthorized("Invalid");
                }
            }
            return Unauthorized("Invalid");
        }


        [HttpGet("getrole")]
        public IActionResult GetRoles()
        {
            var users = _userManager.Users.ToList(); // Get all the users

            var result = new List<object>();

            foreach (var user in users)
            {
                var userRoles = _userManager.GetRolesAsync(user).Result; // Get roles for each user

                foreach (var roleName in userRoles)
                {
                    if (roleName != "Admin") // Check if the role is not 'Admin'
                    {
                        result.Add(new { Id = user.Id, Name = user.FirstName+" " + user.LastName }); ; // Add userId and roleName to the result list
                    }
                }
            }

            return Ok(result); // Return the list of roles and their corresponding IDs
        }

        //[HttpGet("getusers")]
        //public IActionResult GetUsers()
        //{
        //    var users = _userManager.Users.ToList(); // Get all the users

        //    var result = new List<object>();

        //    foreach (var user in users)
        //    {
        //        // Check if the user is not a team lead
        //        if (!user.IsTeamLead)
        //        {
        //            result.Add(new { Id = user.Id, Name = $"{user.FirstName} {user.LastName}" });
        //        }
        //    }

        //    return Ok(result); // Return the list of users and their corresponding IDs
        //}








        private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
        {
            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));
            return token;
        }

        

        [HttpPost]
        [Route("{userId}/upload")]

        public async Task<IActionResult> UploadImage([FromRoute] string userId, IFormFile profileImage)
        {
            var validextension = new List<string>
            {
                ".jpeg",
                ".jpg",
                ".png"
            };

            if (profileImage != null && profileImage.Length > 0)
            {
                if (await task.Exists(userId))
                {
                    var extension = Path.GetExtension(profileImage.FileName);

                    if (validextension.Contains(extension))
                    {
                        var filename = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);

                        var fileImagePath = await imageRepository.Upload(profileImage, filename);

                        if (await task.UpdateProfileImage(userId, fileImagePath))
                        {
                            return Ok(fileImagePath);
                        }

                        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
                    }
                }
                return BadRequest("This is not valid image format");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{userId:guid}")]

        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var user = await task.GetUserAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("updateprofile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdate model)
        {
            if (model.UserId == null)
            {
                return BadRequest("User ID is null.");
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Mobile = model.Mobile;
            user.City = model.City;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Profile Updated Successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Profile update failed!" });
            }
        }

      

    }
}
