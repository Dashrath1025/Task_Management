namespace Task_Management.Models
{
    public class TeamMemberRequest
    {
        public int TeamId { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
