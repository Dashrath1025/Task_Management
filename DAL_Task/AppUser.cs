using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DAL_Task
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; } =string.Empty;

        [Required]
        public string LastName { get; set; }=string.Empty;
         
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB {  get; set; }
        [Required]
        public string? Gender { get; set; }

        [Required]
        [Display(Name ="Mobile No.")]
        public int Mobile { get; set; }
        [Required]
        public string? City { get; set; }
        public string? Image { get; set; }

        [NotMapped]
        public string RoleId { get; set; }= string.Empty;
        [NotMapped]
        public string Role { get; set; }=string.Empty;
      //  [NotMapped]
      //  public IEnumerable<SelectListItem> RoleList { get; set; }

         
    }
}
