using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Task_Management.Models
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage ="User name is required")]
       public string UserName { get; set; }= string.Empty;
        [EmailAddress]
        public string Email { get; set; }=string.Empty;

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Mobile No.")]
        public int Mobile { get; set; }
        [Required]
        public string City { get; set; }= string.Empty;

      //  public string RoleSelected { get; set; }=string.Empty;

     //   public IEnumerable<SelectListItem> RoleList { get; set; }


    }
}
