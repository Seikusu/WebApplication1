using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Experience
    {
        public int ExperienceID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PersonID { get; set; }

        public virtual Person Person { get; set; }
    }
}