using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace eStoreClient.Controllers
{
    public class MembersController : Controller
    {
        private readonly HttpClient client = null;
        private string BaseAddressURI = "https://localhost:5545/";

        public MembersController()
        {
            client = new HttpClient();
            
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            BaseAddressURI = "https://localhost:5545/";
            
        }

        // GET: Members
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Đặt URL của API
            client.BaseAddress = new Uri(BaseAddressURI);
            
            // Gọi API và lấy kết quả trả về dưới dạng chuỗi JSON
            var response = await client.GetAsync("api/members");
            var result = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<Member> list = JsonSerializer.Deserialize<List<Member>>(result, options);

            return View(list);
        }

        // GET: Members/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/members/{id}");
            var result = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Member member = JsonSerializer.Deserialize<Member>(result, options);

            return View(member);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,Email,CompanyName,City,Country,Password")] Member member)
        {
            if (ModelState.IsValid)
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var data = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/members", data);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Insert successfully!";
                }
                else
                {
                    ViewBag.Message = "Error while calling WebAPI!";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/members/{id}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Member member = JsonSerializer.Deserialize<Member>(result, options);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,Email,CompanyName,City,Country,Password")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.BaseAddress = new Uri(BaseAddressURI);
                    var data = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"api/members/{id}", data);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/members/{id}");
            var result = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Member member = JsonSerializer.Deserialize<Member>(result, options);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.DeleteAsync($"api/members/{id}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
