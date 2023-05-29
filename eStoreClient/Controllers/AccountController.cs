using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using BusinessObject;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace YourApplication.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly HttpClient client = null;
        private string BaseAddressURI = "https://localhost:5545/";
        public AccountController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            BaseAddressURI = "https://localhost:5545/";
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                email = email,
                password = password
            }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth", content);
            try
            {
                response.EnsureSuccessStatusCode();
            } catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }

            var result = JsonConvert.DeserializeObject<UserClaims>(await response.Content.ReadAsStringAsync());
            
            // Sign in the user
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, result.Role),
            };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            // Add a cookie with the member ID
                var response2 = await client.GetAsync("api/members");
            try
            {
                response2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            if (result.Role != "Admin")
            {
                var result2 = JsonConvert.DeserializeObject<List<Member>>(await response2.Content.ReadAsStringAsync());
                    var memberId = result2.Where(m => m.Email == email).FirstOrDefault().MemberId;
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7)
                    };
                    Response.Cookies.Append("MemberId", memberId.ToString(), cookieOptions);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
