namespace Task_Management.Models
{
    public class UserResponse
    {
        public string UserId { get; set; }

        public string Email { get; set; }

        public string RoleId { get; set; }

        public string Name { get; set; }    

        public string Role { get; set; }

        public List<string> Rolelist { get; set; }
    }
}
