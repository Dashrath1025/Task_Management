using System.ComponentModel.DataAnnotations;

namespace DAL_Task.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string Title { get; set; }=string.Empty;
        [Required]


        public string Description { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public string Status { get; set; }= string.Empty;
        [Required]
        public string? AssigneeId { get; set; }

    }
}
