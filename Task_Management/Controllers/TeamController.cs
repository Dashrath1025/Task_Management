using BLL_Task;
using BLL_Task.Repositories;
using DAL_Task;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _team;
        private readonly ITeamMember _teamMember;
        private readonly UserManager<AppUser> _userManager;




        public TeamController(ITeamRepository team, AppDbContext dbContext, ITeamMember teamMember, UserManager<AppUser> userManager)
        {
            _team = team;
            _teamMember = teamMember;
            _userManager = userManager;
        }

        [HttpPost] 
        [Route("create-team")]
        public async Task<IActionResult> CreateTeam(Team model)
        {


            try
            {
                int newTeamId = await _team.CreateTeamAsync(model);
                return Ok(new { TeamId = newTeamId, Message = "Team created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating team: {ex.Message}");
            }

        }


        [HttpPost]
        [Route("add-member")]

        public async Task AddMembersToTeamAsync(TeamMemberRequest request)
        {
            // Validate the request, check if the team and members exist, etc.
            // You'll need to implement this based on your business logic.

            foreach (var memberId in request.MemberIds)
            {
                // Create a new TeamMember instance
                var teamMember = new TeamMember
                {
                    TId = request.TeamId,
                    MemberId = memberId
                };

                // Add the TeamMember to the repository
                await _teamMember.AddTeamMemberAsync(teamMember);
            }

            // Save changes to the database
            // This assumes your repository is using an underlying DbContext or similar unit of work
        }
    



    [HttpGet("{teamId}")]
        public async Task<IActionResult> GetTeam(int teamId)
        {
            try
            {
                Team team = await _team.GetTeamByIdAsync(teamId);

                if (team == null)
                {
                    return NotFound($"Team with ID {teamId} not found.");
                }

                return Ok(team);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving team: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("getteams")]

        public IEnumerable<Team> GetTeams()
        {
            return _team.GetTeams();
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
                        result.Add(new { Id = user.Id, Name = user.FirstName + " " + user.LastName }); ; // Add userId and roleName to the result list
                    }
                }
            }

            return Ok(result); // Return the list of roles and their corresponding IDs
        }

    }
}
