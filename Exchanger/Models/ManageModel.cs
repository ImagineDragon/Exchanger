using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Newtonsoft.Json.Serialization;

namespace Exchanger.Models
{
    public class EditUserModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Parent Name")]
        public string ParentName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Display(Name = "Files")]
        public int Files { get; set; }

        //[Display(Name = "User Type")]
        //public int UserType { get; set; }

        [Display(Name = "User Type")]
        public List<string> UserType { get; set; }
    }

    public class ShowEditUserModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Parent Name")]
        public string ParentName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Display(Name = "Files")]
        public int Files { get; set; }

        //[Display(Name = "User Type")]
        //public int UserType { get; set; }

        [Display(Name = "User Type")]
        public string UserType { get; set; }
    }

    public class DisplayUserModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Parent Name")]
        public string ParentName { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Files")]
        public int Files { get; set; }
        
        [Display(Name = "User Type")]
        public string UserType { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Old password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimal length is 6")]
        [Display(Name="New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name="Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class RestorePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="New password")]
        [MinLength(6, ErrorMessage = "Minimal length is 6")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}