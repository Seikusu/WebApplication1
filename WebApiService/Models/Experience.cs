using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApiService.Models
{
    public class Experience
    {
        public int ExperienceID { get; set; }  // Pastikan ini bertipe int
        public int PersonID { get; set; }  // Harus bertipe int jika PersonID di database juga int
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual Person Person { get; set; }
    }
}