using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Person : IValidatableObject
    {
        public int PersonID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        // Properti baru yang ditambahkan
        [Required]
        public string Alamat { get; set; }
        [Required]
        [Phone]
        public string NomorTelepon { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }

        // Add additional properties if needed

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var db = new PeopleDbContext(); // Get the DbContext instance

            // Check if email already exists
            bool emailExists = db.People.Any(p => p.Email == Email && p.PersonID != PersonID);
            bool nomorTeleponExists = db.People.Any(p => p.NomorTelepon == NomorTelepon && p.PersonID != PersonID);
            if (emailExists)
            {
                yield return new ValidationResult("An account with this email address already exists.", new[] { "Email" });
            }
            if (nomorTeleponExists)
            {
                yield return new ValidationResult("An account with this phone number already exists.", new[] { "NomorTelepon" });
            }
        }
    }

}

