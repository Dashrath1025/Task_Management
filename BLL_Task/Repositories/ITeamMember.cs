using DAL_Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Task.Repositories
{
    public interface ITeamMember
    {
        Task AddTeamMemberAsync(TeamMember teamMember);

    }
}
