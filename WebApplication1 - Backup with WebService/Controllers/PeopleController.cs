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
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace WebApplication1.Controllers
{
    public class PeopleController : Controller
    {
        private PeopleDbContext db = new PeopleDbContext(); // Inisialisasi DbContext
        private readonly HttpClient _httpClient;

        public PeopleController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44320/api/");
        }

        // GET: People (Read All)
        public async Task<ActionResult> Index()
        {
            IEnumerable<Person> peopleList = new List<Person>();
            string errorMessage = string.Empty;

            try
            {
                // Ambil data dari API
                var response = await _httpClient.GetAsync("PeopleApi");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    peopleList = JsonConvert.DeserializeObject<IEnumerable<Person>>(jsonResponse);
                }
                else
                {
                    errorMessage = "Gagal terhubung dengan WebService. Status code: " + response.StatusCode;
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                // Tangani kesalahan terkait HTTP
                errorMessage = "Kesalahan saat menghubungi WebService: " + httpRequestException.Message;
            }
            catch (Exception ex)
            {
                // Tangani kesalahan lainnya
                errorMessage = "Terjadi kesalahan: " + ex.Message;
            }

            // Kirimkan data dan pesan error ke view
            ViewBag.ErrorMessage = errorMessage;
            return View(peopleList);
        }


        // GET: People/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"PeopleApi/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(jsonResponse);
                return View(person);
            }
            return HttpNotFound();
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create (Create Person)
        [HttpPost]
        public async Task<ActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(person);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("PeopleApi", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"PeopleApi/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(jsonResponse);
                return View(person);
            }
            return HttpNotFound();
        }

        // POST: People/Edit/5 (Update Person)
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Person person)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(person);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"PeopleApi/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"PeopleApi/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(jsonResponse);
                return View(person);
            }
            return HttpNotFound();
        }

        // POST: People/Delete/5 (Delete Person)
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"PeopleApi/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddExperience(int id) // id adalah PersonID
        {
            var experience = new Experience
            {
                PersonID = id // Pastikan PersonID dikirim ke form
            };
            return View(experience); // Kirim model ke view AddExperience.cshtml
        }

        [HttpPost]
        public async Task<ActionResult> AddExperience(Experience experience)
        {
            Debug.WriteLine($"PersonID: {experience.PersonID}, Title: {experience.Title}, Description: {experience.Description}");
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:44320/api/");

                    // Konversi model ke JSON dan kirimkan ke Web API
                    var content = new StringContent(JsonConvert.SerializeObject(experience), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("ExperienceApi", content); // ExperienceApi adalah endpoint di Web API

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Details", new { id = experience.PersonID }); // Kembali ke halaman detail orang
                    }
                }
            }
            return View(experience);
        }
        [HttpGet]
        public async Task<ActionResult> EditExperience(int ids) // id adalah ExperienceID
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44320/api/");

                // Ambil data pengalaman dari Web API berdasarkan ExperienceID
                var response = await httpClient.GetAsync($"ExperienceApi/{ids}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var experience = JsonConvert.DeserializeObject<Experience>(jsonResponse);
                    return View(experience); // Kirim data ke view EditExperience.cshtml
                }
            }

            return HttpNotFound();
        }
        [HttpPost]
        public async Task<ActionResult> EditExperience(Experience experience)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://localhost:44320/api/");

                    // Konversi model ke JSON dan kirimkan ke Web API
                    var content = new StringContent(JsonConvert.SerializeObject(experience), Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync($"ExperienceApi/{experience.ExperienceID}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "People");
                    }
                }
            }

            return View(experience);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteExperience(int ids) // id adalah ExperienceID
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44320/api/");

                // Ambil data pengalaman dari Web API berdasarkan ExperienceID
                var response = await httpClient.GetAsync($"ExperienceApi/{ids}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var experience = JsonConvert.DeserializeObject<Experience>(jsonResponse);
                    return View(experience); // Kirim data ke view DeleteExperience.cshtml
                }
                else
                {
                    // Tangani kasus ketika data tidak ditemukan atau response gagal
                    return HttpNotFound();
                }
            }
        }


        [HttpPost, ActionName("DeleteExperienceConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteExperienceConfirmed(int ExperienceID)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:44320/api/");

                var response = await httpClient.DeleteAsync($"ExperienceApi/{ExperienceID}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Gagal menghapus pengalaman.");
                    return View();
                }
            }
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
