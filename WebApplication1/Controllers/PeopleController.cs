using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PeopleController : Controller
    {
        private readonly HttpClient _httpClient;

        public PeopleController()
        {
            _httpClient = new HttpClient();
            string apiBaseUrl = WebConfigurationManager.AppSettings["ApiBaseUrl"];
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
        }

        // GET: People (Read All)
        public async Task<ActionResult> Index()
        {
            IEnumerable<Person> peopleList = new List<Person>();
            string errorMessage = string.Empty;
            string apiUrl = WebConfigurationManager.AppSettings["PeopleApiIndex"];

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
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
            catch (HttpRequestException ex)
            {
                errorMessage = "Kesalahan saat menghubungi WebService: " + ex.Message;
            }
            catch (Exception ex)
            {
                errorMessage = "Terjadi kesalahan: " + ex.Message;
            }

            ViewBag.ErrorMessage = errorMessage;
            return View(peopleList);
        }

        // GET: People/Details/5
        public async Task<ActionResult> Details(int id)
        {
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["PeopleApiDetails"], id);
            var response = await _httpClient.GetAsync(apiUrl);
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

        // POST: People/Create
        [HttpPost]
        public async Task<ActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = WebConfigurationManager.AppSettings["PeopleApiCreate"];
                var jsonContent = JsonConvert.SerializeObject(person);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, content);

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
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["PeopleApiEdit"], id);
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(jsonResponse);
                return View(person);
            }
            return HttpNotFound();
        }

        // POST: People/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Person person)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = string.Format(WebConfigurationManager.AppSettings["PeopleApiEdit"], id);
                var jsonContent = JsonConvert.SerializeObject(person);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(apiUrl, content);

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
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["PeopleApiDelete"], id);
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<Person>(jsonResponse);
                return View(person);
            }
            return HttpNotFound();
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["PeopleApiDelete"], id);
            var response = await _httpClient.DeleteAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Experience/Add
        [HttpGet]
        public ActionResult AddExperience(int id)
        {
            var experience = new Experience
            {
                PersonID = id
            };
            return View(experience);
        }

        // POST: Experience/Add
        [HttpPost]
        public async Task<ActionResult> AddExperience(Experience experience)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = WebConfigurationManager.AppSettings["ExperienceApiAdd"];
                var jsonContent = JsonConvert.SerializeObject(experience);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", new { id = experience.PersonID });
                }
            }
            return View(experience);
        }

        // GET: Experience/Edit/5
        [HttpGet]
        public async Task<ActionResult> EditExperience(int ids)
        {
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["ExperienceApiDetails"], ids);
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var experience = JsonConvert.DeserializeObject<Experience>(jsonResponse);
                return View(experience);
            }
            return HttpNotFound();
        }

        // POST: Experience/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditExperience(Experience experience)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = string.Format(WebConfigurationManager.AppSettings["ExperienceApiEdit"], experience.ExperienceID);
                var jsonContent = JsonConvert.SerializeObject(experience);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(experience);
        }

        // GET: Experience/Delete/5
        [HttpGet]
        public async Task<ActionResult> DeleteExperience(int ids)
        {
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["ExperienceApiDelete"], ids);
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var experience = JsonConvert.DeserializeObject<Experience>(jsonResponse);
                return View(experience);
            }
            return HttpNotFound();
        }

        // POST: Experience/Delete/5
        [HttpPost, ActionName("DeleteExperienceConfirmed")]
        public async Task<ActionResult> DeleteExperienceConfirmed(int experienceID)
        {
            string apiUrl = string.Format(WebConfigurationManager.AppSettings["ExperienceApiDelete"], experienceID);
            var response = await _httpClient.DeleteAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
