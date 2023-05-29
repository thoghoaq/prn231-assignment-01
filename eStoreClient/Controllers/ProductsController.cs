using BusinessObject;
using eStoreClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eStoreClient.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string BaseAddressURI = "https://localhost:5545/";

        public ProductsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            BaseAddressURI = "https://localhost:5545/";
        }

        // GET: ProductController
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Đặt URL của API
            client.BaseAddress = new Uri(BaseAddressURI);

            // Gọi API và lấy kết quả trả về dưới dạng chuỗi JSON
            var response = await client.GetAsync("api/Products");
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
            List<Product> listProducts = JsonSerializer.Deserialize<List<Product>>(result, options);

            var res = await client.GetAsync("api/Categories");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Category> categories = JsonSerializer.Deserialize<List<Category>>(resu, options);
            foreach (Product p in listProducts)
            {
                p.Category = categories.Where(c => c.CategoryId == p.CategoryId).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                listProducts = listProducts.Where(p => p.ProductName.Contains(searchTerm) ||
                                               p.UnitPrice.ToString().Contains(searchTerm))
                                  .ToList();
            }

            return View(listProducts);
        }

        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/Products/{id}");
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
            Product product = JsonSerializer.Deserialize<Product>(result, options);

            var res = await client.GetAsync($"api/Categories/{product.CategoryId}");
            var resu = await res.Content.ReadAsStringAsync();
            Category category = JsonSerializer.Deserialize<Category>(resu, options);
            product.Category = category;

            return View(product);
        }

        // GET: ProductController/Create
        public async Task<IActionResult> Create()
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            ViewData["CategoryId"] = new SelectList(await this.GetCategoriesAsync(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,CategoryId,ProductName,Weight,UnitPrice,UnitsInStock")] Product product)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            if (ModelState.IsValid)
            {
                var data = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/products", data);
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

            ViewData["CategoryId"] = new SelectList(await this.GetCategoriesAsync(), "CategoryId", "CategoryName");
            return View(product);
        }

        // GET: ProductController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/Products/{id}");
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
            Product product = JsonSerializer.Deserialize<Product>(result, options);

            var res = await client.GetAsync($"api/Categories/{product.CategoryId}");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            Category category = JsonSerializer.Deserialize<Category>(resu, options);
            product.Category = category;

            ViewData["CategoryId"] = new SelectList(await this.GetCategoriesAsync(), "CategoryId", "CategoryName");

            return View(product);
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,CategoryId,ProductName,Weight,UnitPrice,UnitsInStock")] Product product)
        {
            try
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var data = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/products/{id}", data);
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
                ViewData["CategoryId"] = new SelectList(await this.GetCategoriesAsync(), "CategoryId", "CategoryName");
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            client.BaseAddress = new Uri(BaseAddressURI);
            var response = await client.GetAsync($"api/Products/{id}");
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
            Product product = JsonSerializer.Deserialize<Product>(result, options);

            var res = await client.GetAsync($"api/Categories/{product.CategoryId}");
            var resu = await res.Content.ReadAsStringAsync();
            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            Category category = JsonSerializer.Deserialize<Category>(resu, options);
            product.Category = category;

            return View(product);
        }

        // POST: ProductController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                client.BaseAddress = new Uri(BaseAddressURI);
                var response = await client.DeleteAsync($"api/products/{id}");
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

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var res = await client.GetAsync("api/Categories");
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
            List<Category> categories = JsonSerializer.Deserialize<List<Category>>(resu, options);
            return categories;
        }
    }
}
