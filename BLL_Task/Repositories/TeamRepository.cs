
using DAL_Task;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Task
{
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _db;

        public TeamRepository(AppDbContext db)
        {
                _db = db;
        }

        public async Task<int> CreateTeamAsync(Team team)
        {
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();
            return team.TeamId;
        }

        public async Task<Team> GetTeamByIdAsync(int teamId)
        {
            return await _db.Teams.FindAsync(teamId);
        }

        public IEnumerable<Team> GetTeams()
        {
            return _db.Teams.ToList();
        }
    }
}
