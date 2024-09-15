using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiService.Models;

namespace WebApiService.Controllers
{
    [RoutePrefix("api/AccountApi")]

    public class AccountApiController : ApiController
    {
        private PeopleDbContext db = new PeopleDbContext();
        // POST: api/AccountApi/Login

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody] Users model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Hash password yang dimasukkan (jika digunakan)
                    //var hashedPassword = HashPassword(model.Password);

                    // Cari pengguna di database berdasarkan Username dan Password yang di-hash
                    var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                    if (user != null)
                    {
                        // Update status login ke TRUE
                        user.IsLoggedIn = true;
                        db.SaveChanges();

                        return Ok(new { message = "Login successful" });
                    }
                    else
                    {
                        // Jika Username atau Password salah
                        return Unauthorized();
                    }
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log kesalahan
                Debug.WriteLine("Error: " + ex.Message);
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout([FromBody] Users model)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == model.Username);

            if (user != null)
            {
                // Update status login ke FALSE
                user.IsLoggedIn = false;
                db.SaveChanges();
                return Ok(new { message = "Logout successful" });
            }

            return BadRequest("User not found.");
        }


        // Example hashing function, replace with your actual hashing method
        private string HashPassword(string password)
        {
            // Hash password (this is just an example)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return string.Concat(bytes.Select(b => b.ToString("x2")));
            }
        }

    }

}