namespace Task_Management.Models
{
    public class UserProfileUpdate
    {
        public string UserId { get; set; }
        public int Mobile { get; set; }
        public string City { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
