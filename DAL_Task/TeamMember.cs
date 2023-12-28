using DAL_Task;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TeamMember
{
    [Key]
    public int TeamMemberId { get; set; }

    public int TId { get; set; }

    [ForeignKey("TId")]
    public Team Team { get; set; }

    public string MemberId { get; set; }

   
}
