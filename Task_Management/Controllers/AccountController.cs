using BLL_Task;
using DAL_Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.WebPages;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITask task;
        private readonly IImageRepository imageRepository;


        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ITask task, IImageRepository imageRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            this.task = task;
            this.imageRepository = imageRepository;
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


            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Already Exist!" });

            }


            AppUser user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
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
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
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
            return Unauthorized();
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
                        result.Add(new { Id = user.Id, Name = user.UserName }); // Add userId and roleName to the result list
                    }
                }
            }

            return Ok(result); // Return the list of roles and their corresponding IDs
        }




        //private string GenerateJwtToken(Register user)
        //{
        //    var tokenhandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
        //        {
        //            new Claim("email", user.Email),
        //            new Claim("userName", user.UserName),
        //            new Claim("dob", user.DOB.ToString()),
        //            new Claim("mobile", user.Mobile.ToString()),
        //            new Claim("gender", user.Gender),
        //            new Claim("city", user.City),
        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(10),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        //    };
        //    var token = tokenhandler.CreateToken(tokenDescriptor);
        //    return tokenhandler.WriteToken(token);
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

        //[HttpPut("updateprofile")]

        //public async Task<IActionResult> UpdateUserProfile([FromForm] UserProfileUpdate model)
        //{
        //    var user = await _userManager.FindByNameAsync(User?.Identity?.Name);

        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    var files= HttpContext.Request.Form.Files;
        //    string webRootPath= _webHostEnvironment.WebRootPath;

        //    string upload = webRootPath + WC.ImagePath;
        //    var filename = Guid.NewGuid().ToString();
        //    string extension = Path.GetExtension(files[0].FileName);

        //    using(var filestream= new FileStream(Path.Combine(upload, filename + extension), FileMode.Create))
        //    {
        //        files[0].CopyTo(filestream);
        //    }

        //    model.Image = filename + extension;

        //    user.Mobile = model.Mobile;
        //    user.City= model.City;

        //    var result = await _userManager.UpdateAsync(user);

        //    if (result.Succeeded)
        //    {
        //        return Ok(new Response { Status = "Success", Message = "Profile Updated Successfully" });
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Profile update failed!" });
        //    }

        //}

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

            if(user == null)
            {
                    return NotFound();
            }

            return Ok(user);
        }

    }
}
