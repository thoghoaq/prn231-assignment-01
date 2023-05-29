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
    public class OrderDetailsController : Controller
    {
        private readonly HttpClient client = null;
        private string BaseAddressURI = "https://localhost:5545/";

        public OrderDetailsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            BaseAddressURI = "https://localhost:5545/";
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            // Đặt URL của API
            client.BaseAddress = new Uri(BaseAddressURI);

            // Gọi API và lấy kết quả trả về dưới dạng chuỗi JSON
            var response = await client.GetAsync("api/orderdetails");
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
            List<OrderDetail> list = JsonSerializer.Deserialize<List<OrderDetail>>(result, options);

            var res = await client.GetAsync("api/Orders");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Order> orders = JsonSerializer.Deserialize<List<Order>>(resu, options);
            foreach (OrderDetail p in list)
            {
                p.Order = orders.Where(c => c.OrderId == p.OrderId).FirstOrDefault();
            }

            var res2 = await client.GetAsync("api/Products");
            var resu2 = await res2.Content.ReadAsStringAsync();
            try
            {
                res2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Product> products = JsonSerializer.Deserialize<List<Product>>(resu2, options);
            foreach (OrderDetail p in list)
            {
                p.Product = products.Where(c => c.ProductId == p.ProductId).FirstOrDefault();
            }

            return View(list);
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orderdetails/{id}");
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
            List<OrderDetail> list = JsonSerializer.Deserialize<List<OrderDetail>>(result, options);

            var res = await client.GetAsync("api/Orders");
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            var resu = await res.Content.ReadAsStringAsync();
            List<Order> orders = JsonSerializer.Deserialize<List<Order>>(resu, options);
            foreach (OrderDetail p in list)
            {
                p.Order = orders.Where(c => c.OrderId == p.OrderId).FirstOrDefault();
            }

            var res2 = await client.GetAsync("api/Products");
            var resu2 = await res2.Content.ReadAsStringAsync();
            try
            {
                res2.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Product> products = JsonSerializer.Deserialize<List<Product>>(resu2, options);
            foreach (OrderDetail p in list)
            {
                p.Product = products.Where(c => c.ProductId == p.ProductId).FirstOrDefault();
            }

            return View(list);
        }

        // GET: OrderDetails/Create
        public async Task<IActionResult> Create()
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            ViewData["OrderId"] = new SelectList(await this.GetOrdersAsync(), "OrderId", "OrderId");
            ViewData["ProductId"] = new SelectList(await this.GetProductsAsync(), "ProductId", "ProductName");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,UnitPrice,Quantity,Discount")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var data = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/orderdetails", data);
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
            ViewData["OrderId"] = new SelectList(await this.GetOrdersAsync(), "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(await this.GetProductsAsync(), "ProductId", "ProductName", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orderdetails/{id}");
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
            OrderDetail orderDetail = JsonSerializer.Deserialize<OrderDetail>(result, options);

            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(await this.GetOrdersAsync(), "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(await this.GetProductsAsync(), "ProductId", "ProductName", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ProductId,UnitPrice,Quantity,Discount")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.BaseAddress = new Uri(BaseAddressURI);
                    var data = new StringContent(JsonConvert.SerializeObject(orderDetail), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"api/orderdetails/{id}", data);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Error", "Home");
                    }
                    response.EnsureSuccessStatusCode();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                }
            }
            ViewData["OrderId"] = new SelectList(await this.GetOrdersAsync(), "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["ProductId"] = new SelectList(await this.GetProductsAsync(), "ProductId", "ProductName", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/orderdetails/{id}");
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
            OrderDetail orderDetail = JsonSerializer.Deserialize<OrderDetail>(result, options);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var response = await client.DeleteAsync($"api/orderdetails/{id}");
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
            catch
            {
                return View();
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var res = await client.GetAsync("api/orders");
            var resu = await res.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            List<Order> orders = JsonSerializer.Deserialize<List<Order>>(resu, options);
            return orders;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var res = await client.GetAsync("api/products");
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
            List<Product> products = JsonSerializer.Deserialize<List<Product>>(resu, options);
            return products;
        }
    }
}
