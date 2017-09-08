using System;
using System.ComponentModel.DataAnnotations;

namespace Exchanger.DB.Metadata
{
    public class UserMetadata
    {
        public Guid Id { get; }

        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "Length must be < 50")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Length must be < 50")]
        public string LastName { get; set; }

        [Display(Name = "Parent Name")]
        [StringLength(50, ErrorMessage = "Length must be < 50")]
        public string ParentName { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Login")]
        [StringLength(50, ErrorMessage = "Length must be < 50")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Length must be > 6")]
        public string Password { get; set; }

        [Display(Name = "User Type Id")]
        public int UserTypeId { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Address Id")]
        public Guid AddressId { get; set; }
    }
}