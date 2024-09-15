using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiService.Models
{
    public class Person
    {
        public int PersonID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Alamat { get; set; }
        public string NomorTelepon { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
    }
}