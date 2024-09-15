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
    public class ExperienceApiController : ApiController
    {
        private PeopleDbContext db = new PeopleDbContext();

        // GET: api/ExperienceApi
        [HttpGet]
        public IQueryable<Experience> GetExperiences()
        {
            return db.Experiences;
        }

        // GET: api/ExperienceApi/5
        [HttpGet]
        public IHttpActionResult GetExperience(int id)
        {
            Debug.WriteLine($"ID yang diterima: {id}");

            var experience = db.Experiences.Find(id);

            if (experience == null)
            {
                Debug.WriteLine("Experience tidak ditemukan.");
                return NotFound();
            }

            return Ok(experience);
        }

        // POST: api/ExperienceApi
        [HttpPost]
        public IHttpActionResult PostExperience([FromBody] Experience experience)
        {
            if (experience == null)
            {
                return BadRequest("Data pengalaman yang diterima kosong.");
            }

            // Debugging untuk memeriksa nilai PersonID yang diterima
            Debug.WriteLine($"PersonID: {experience.PersonID}, Title: {experience.Title}, Description: {experience.Description}");

            try
            {
                // Validasi PersonID yang diterima
                var person = db.People.FirstOrDefault(p => p.PersonID == experience.PersonID);
                if (person == null)
                {
                    Debug.WriteLine("PersonID tidak ditemukan di database.");
                    return BadRequest("PersonID tidak valid.");
                }

                if (!ModelState.IsValid)
                {
                    Debug.WriteLine("ModelState tidak valid.");
                    return BadRequest(ModelState);
                }

                // Tambahkan pengalaman ke database
                db.Experiences.Add(experience);
                db.SaveChanges();

                Debug.WriteLine("Pengalaman berhasil disimpan.");
                return CreatedAtRoute("DefaultApi", new { id = experience.ExperienceID }, experience);
            }
            catch (Exception ex)
            {
                // Log kesalahan untuk debugging lebih lanjut
                Debug.WriteLine($"Error: {ex.Message}");
                return InternalServerError(ex);
            }
        }


        // PUT: api/ExperienceApi/5
        [HttpPut]
        public IHttpActionResult PutExperience(int id, [FromBody] Experience experience)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingExperience = db.Experiences.Find(id);
            if (existingExperience == null)
            {
                return NotFound();
            }

            // Perbarui data yang ada dengan data dari request
            existingExperience.Title = experience.Title;
            existingExperience.Description = experience.Description;
            existingExperience.PersonID = experience.PersonID; // Jika perlu diperbarui

            db.SaveChanges(); // Simpan perubahan ke database

            return StatusCode(HttpStatusCode.NoContent); // Kembalikan respons 204 (No Content) setelah berhasil diupdate
        }

        // DELETE: api/ExperienceApi/5
        [HttpDelete]
        public IHttpActionResult DeleteExperience(int id)
        {
            var experience = db.Experiences.Find(id);
            if (experience == null)
            {
                return NotFound();
            }

            db.Experiences.Remove(experience);
            db.SaveChanges();

            return Ok(experience);
        }

        private bool ExperienceExists(int id)
        {
            return db.Experiences.Count(e => e.ExperienceID == id) > 0;
        }
    }

}