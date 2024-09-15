using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController()
        {
            _httpClient = new HttpClient();
            string apiBaseUrl = WebConfigurationManager.AppSettings["ApiBaseUrl"];
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login (Authenticate User via API)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Users model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Konversi data model ke format JSON untuk dikirim ke Web API
                    var jsonContent = JsonConvert.SerializeObject(new
                    {
                        Username = model.Username,
                        Password = model.Password
                    });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Logging data yang dikirim ke Web API
                    Debug.WriteLine("Request yang dikirim ke Web API:");
                    Debug.WriteLine(jsonContent);  // Log request JSON

                    // Ambil URL login dari web.config
                    string apiUrl = WebConfigurationManager.AppSettings["AccountApiLogin"];

                    // Kirim POST request ke Web API login endpoint
                    var response = await _httpClient.PostAsync(apiUrl, content);

                    // Log status code dari Web API
                    Debug.WriteLine($"Status code dari Web API: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Login berhasil, set cookie autentikasi
                        FormsAuthentication.SetAuthCookie(model.Username, false); // false = tidak ingat login

                        // Redirect ke halaman yang dituju (ReturnUrl)
                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "People");
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Tangani kesalahan komunikasi dengan Web API
                    Debug.WriteLine("Error communicating with the API: " + ex.Message);
                    ModelState.AddModelError("", "Error communicating with the API: " + ex.Message);
                }
            }

            // Jika ada kesalahan, tampilkan kembali halaman login dengan pesan error
            return View(model);
        }

        // GET: Account/Logout
        public async Task<ActionResult> Logout()
        {
            if (Session["Username"] != null)
            {
                var username = Session["Username"].ToString();
                var user = new Users { Username = username };

                var jsonContent = JsonConvert.SerializeObject(user);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Debug.WriteLine("Request yang dikirim ke Web API:");
                Debug.WriteLine(jsonContent);  // Log request JSON

                // Ambil URL logout dari web.config
                string apiUrl = WebConfigurationManager.AppSettings["AccountApiLogout"];

                // Kirim POST request ke API logout endpoint
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Session["Username"] = null; // Hapus session
                }

                return RedirectToAction("Login");
            }

            return RedirectToAction("Login");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Username"] == null && !filterContext.ActionDescriptor.ActionName.Equals("Login"))
            {
                filterContext.Result = RedirectToAction("Login");
            }

            base.OnActionExecuting(filterContext);
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
