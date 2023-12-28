using System.ComponentModel.DataAnnotations;

namespace DAL_Task
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        public string TeamName { get; set; }
  
        public string TeamLeadId { get; set; }

     
    }
}
