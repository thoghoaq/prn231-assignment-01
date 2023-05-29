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

namespace eStoreClient.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class OrdersController : Controller
    {
        private readonly HttpClient client = null;
        private string BaseAddressURI = "https://localhost:5545/";

        public OrdersController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            BaseAddressURI = "https://localhost:5545/";
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            // Đặt URL của API
            client.BaseAddress = new Uri(BaseAddressURI);

            // Gọi API và lấy kết quả trả về dưới dạng chuỗi JSON
            var response = await client.GetAsync("api/orders");
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
            List<Order> list = JsonSerializer.Deserialize<List<Order>>(result, options);
            var res = await client.GetAsync("api/Members");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Member> members = JsonSerializer.Deserialize<List<Member>>(resu, options);
            foreach (Order o in list)
            {
                o.Member = members.Where(c => c.MemberId == o.MemberId).FirstOrDefault();
            }
            return View(list);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orders/{id}");
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
            Order order = JsonSerializer.Deserialize<Order>(result, options);

            var res = await client.GetAsync($"api/Members/{order.MemberId}");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            Member member = JsonSerializer.Deserialize<Member>(resu, options);
            order.Member = member;

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            ViewData["MemberId"] = new SelectList(await this.GetMembersAsync(), "MemberId", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (ModelState.IsValid)
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var data = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/orders", data);
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
            ViewData["MemberId"] = new SelectList(await this.GetMembersAsync(), "MemberId", "Email");
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orders/{id}");
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
            Order order = JsonSerializer.Deserialize<Order>(result, options);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(await this.GetMembersAsync(), "MemberId", "Email");
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.BaseAddress = new Uri(BaseAddressURI);
                    var data = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"api/orders/{id}", data);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    response.EnsureSuccessStatusCode();
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(await this.GetMembersAsync(), "MemberId", "Email");
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orders/{id}");
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
            Order order = JsonSerializer.Deserialize<Order>(result, options);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.DeleteAsync($"api/orders/{id}");
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

        public async Task<List<Member>> GetMembersAsync()
        {
            var res = await client.GetAsync("api/Members");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return null;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<Member> members = JsonSerializer.Deserialize<List<Member>>(resu, options);
            return members;
        }
    }
}
