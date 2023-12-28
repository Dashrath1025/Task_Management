using DAL_Task;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Task.Repositories
{
    public class TeamMemberRepository : ITeamMember
    {
        private readonly AppDbContext _db;

        public TeamMemberRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task AddTeamMemberAsync(TeamMember teamMember)
        {
            _db.TeamMembers.Add(teamMember);

            // Save changes to the database
            await _db.SaveChangesAsync();
        }
    }
}
