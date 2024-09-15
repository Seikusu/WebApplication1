using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApiService.Models;

namespace WebApiService.Controllers
{
    public class PeopleApiController : ApiController
    {
        private PeopleDbContext db = new PeopleDbContext();

        // GET: api/PeopleApi
        public IQueryable<Person> GetPeople()
        {
            return db.People.Include(p => p.Experiences);
        }

        // GET: api/PeopleApi/5
        [ResponseType(typeof(Person))]
        public IHttpActionResult GetPerson(int id)
        {
            Log.Information("Received GET request for person ID: {PersonId}", id);
            Person person = db.People.Include(p => p.Experiences).FirstOrDefault(p => p.PersonID == id);
            if (person == null)
            {
                Log.Warning("Person with ID: {PersonId} not found", id);
                return NotFound();
            }
            return Ok(person);
        }

        // POST: api/PeopleApi
        [ResponseType(typeof(Person))]
        public IHttpActionResult PostPerson(Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.People.Add(person);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = person.PersonID }, person);
        }

        // PUT: api/PeopleApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPerson(int id, Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != person.PersonID)
            {
                return BadRequest();
            }

            db.Entry(person).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/PeopleApi/5
        [ResponseType(typeof(Person))]
        public IHttpActionResult DeletePerson(int id)
        {
            Person person = db.People.Include(p => p.Experiences).FirstOrDefault(p => p.PersonID == id);
            if (person == null)
            {
                return NotFound();
            }

            db.People.Remove(person);
            db.SaveChanges();

            return Ok(person);
        }

        private bool PersonExists(int id)
        {
            return db.People.Count(e => e.PersonID == id) > 0;
        }
    }

}