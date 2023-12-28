using DAL_Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Task
{
    public interface ITeamRepository
    {

        IEnumerable<Team> GetTeams();
        Task<Team> GetTeamByIdAsync(int teamId);
        Task<int> CreateTeamAsync(Team team);
    }
}
