using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Exchanger.DB;

namespace Exchanger.Models
{
    public class UsersModel : IValidatableObject
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using (var db = new ExchangedbEntities())
            {
                var results = new List<ValidationResult>();

                Validator.TryValidateProperty(Login, new ValidationContext(this, null, null) { MemberName = "Login" },
                    results);
                Validator.TryValidateProperty(Password,
                    new ValidationContext(this, null, null) { MemberName = "Password" }, results);

                var dbUser = db.Users.FirstOrDefault(a => a.Login.ToLower() == Login.ToLower());

                if (dbUser == null)
                {
                    results.Add(new ValidationResult("Wrong login"));
                }
                else if (dbUser.Password != Password)
                {
                    results.Add(new ValidationResult("Wrong password"));
                }
                return results;
            }
        }
    }
}