using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Diagnostics;


namespace WebApplication1.Controllers
{
    public class PeopleController : Controller
    {
        private PeopleDbContext db = new PeopleDbContext();

        // GET: People
        public ActionResult Index()
        {
            return View(db.People.ToList());
        }

        // GET: People/Details/5
        [Route("People/Details/{id}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Person person)
        {
            // Log the posted person object
            Debug.WriteLine($"Create POST - Name: {person.Name}, Email: {person.Email}, NomorTelepon: {person.NomorTelepon}");

            if (ModelState.IsValid)
            {
                // Check if email already exists
                bool emailExists = db.People.Any(p => p.Email == person.Email);
                var normalizedPhoneNumber = person.NomorTelepon.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                bool nomorTeleponExists = db.People.Any(p => p.NomorTelepon.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") == normalizedPhoneNumber);

                // Log for debugging purposes
                Debug.WriteLine($"Create POST - normalizedPhoneNumber: {normalizedPhoneNumber}");
                Debug.WriteLine($"Create POST - emailExists: {emailExists}, nomorTeleponExists: {nomorTeleponExists}");

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "An account with this email address already exists.");
                    Debug.WriteLine("Create POST - Email already exists.");
                    return View(person);
                }
                if (nomorTeleponExists)
                {
                    ModelState.AddModelError("NomorTelepon", "An account with this phone number already exists.");
                    Debug.WriteLine("Create POST - Phone Number already exists.");
                    return View(person);
                }

                db.People.Add(person);
                db.SaveChanges();
                Debug.WriteLine("Create POST - Person created successfully.");
                return RedirectToAction("Index");
            }

            return View(person);
        }



        // GET: People/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Person person)
        {
            // Log the posted person object
            Debug.WriteLine($"Edit POST - PersonID: {person.PersonID}, Name: {person.Name}, Email: {person.Email}, NomorTelepon: {person.NomorTelepon}");

            if (ModelState.IsValid)
            {
                // Normalize the current phone number to compare it with others in the database
                var normalizedPhoneNumber = person.NomorTelepon.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

                // Check if email already exists, excluding the current person being edited
                bool emailExists = db.People.Any(p => p.Email == person.Email && p.PersonID != person.PersonID);

                // Check if phone number already exists, excluding the current person being edited
                bool nomorTeleponExists = db.People.Any(p => p.NomorTelepon.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") == normalizedPhoneNumber && p.PersonID != person.PersonID);

                // Log for debugging purposes
                Debug.WriteLine($"Edit POST - normalizedPhoneNumber: {normalizedPhoneNumber}");
                Debug.WriteLine($"Edit POST - emailExists: {emailExists}, nomorTeleponExists: {nomorTeleponExists}");

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "An account with this email address already exists.");
                    Debug.WriteLine("Edit POST - Email already exists.");
                    return View(person);
                }

                if (nomorTeleponExists)
                {
                    ModelState.AddModelError("NomorTelepon", "An account with this phone number already exists.");
                    Debug.WriteLine("Edit POST - Phone Number already exists.");
                    return View(person);
                }

                // Mark entity as modified
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();

                Debug.WriteLine("Edit POST - Person updated successfully.");
                return RedirectToAction("Index");
            }

            return View(person);
        }



        // GET: People/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Temukan entri Person dan termasuk referensi ke Experiences
            Person person = db.People.Include(p => p.Experiences).FirstOrDefault(p => p.PersonID == id);

            if (person == null)
            {
                return HttpNotFound();
            }

            // Hapus semua pengalaman yang terkait
            db.Experiences.RemoveRange(person.Experiences);

            // Hapus entri orang
            db.People.Remove(person);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        /*(public ActionResult AddExperience(int personId)
        {
            var experience = new Experience
            {
                PersonID = personId // Set PersonID agar bisa dikirimkan oleh form
            };

            return View(experience);
        }*/

        /*public ActionResult AddExperience(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PersonID = id;
            return View();
        }*/
        [HttpGet]
        public ActionResult AddExperience(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Error"); // Handle jika personId null
            }

            // Isi nilai PersonID di model Experience
            var experience = new Experience
            {
                PersonID = id.Value
            };

            return View(experience); // Kirim model dengan PersonID ke view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExperience([Bind(Include = "Title,Description,PersonID")] Experience experience)
        {
            // Debug output to check if the model is received correctly
            if (experience == null)
            {
                Debug.WriteLine("Experience object is null.");
            }
            else
            {
                Debug.WriteLine("Experience PersonID: " + experience.PersonID);
                Debug.WriteLine("Experience Title: " + experience.Title);
                Debug.WriteLine("Experience Description: " + experience.Description);
            }

            if (ModelState.IsValid)
            {
                db.Experiences.Add(experience);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = experience.PersonID });
            }

            return View(experience);
        }
        [HttpGet]
        /*public ActionResult EditExperience(int ids)
        {
            var experience = db.Experiences.Find(ids);
            if (experience == null)
            {
                return HttpNotFound();
            }

            // Log the retrieved experience object
            Debug.WriteLine($"EditExperience GET - ExperienceID: {experience.ExperienceID}, Title: {experience.Title}, Description: {experience.Description}, PersonID: {experience.PersonID}");

            return View(experience);
        }*/

        public ActionResult EditExperience(int? ids)
        {
            if (ids == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Experience experience = db.Experiences.Find(ids);
            if (experience == null)
            {
                return HttpNotFound();
            }
            return View(experience);
        }

        // POST: People/EditExperience/5
        [HttpPost]
        public ActionResult EditExperience(Experience experience)
        {
            // Log the posted experience object
            Debug.WriteLine($"EditExperience POST - ExperienceID: {experience.ExperienceID}, Title: {experience.Title}, Description: {experience.Description}, PersonID: {experience.PersonID}");

            if (experience == null)
            {
                Debug.WriteLine("EditExperience - Experience object is null.");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Validasi PersonID
            bool validPerson = db.People.Any(p => p.PersonID == experience.PersonID);
            Debug.WriteLine($"EditExperience - PersonID validation result: {validPerson}");

            if (!validPerson)
            {
                ModelState.AddModelError("PersonID", "Invalid PersonID.");
                Debug.WriteLine("EditExperience - Invalid PersonID.");
                return View(experience);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(experience).State = EntityState.Modified;
                    db.SaveChanges();
                    Debug.WriteLine("EditExperience - Data updated successfully.");

                    // Redirect with route parameter
                    return RedirectToAction("Index", "People");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"EditExperience - Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating the experience.");
                }
            }
            else
            {
                Debug.WriteLine("EditExperience - ModelState is not valid.");
            }

            return View(experience);
        }

        // POST: People/DeleteExperience
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExperience(int? ids)
        {
            var experience = db.Experiences.Find(ids);
            if (experience != null)
            {
                db.Experiences.Remove(experience);
                db.SaveChanges();
            }
            //return RedirectToAction("Index", "People");
            return RedirectToAction("Details", new { id = experience.PersonID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
