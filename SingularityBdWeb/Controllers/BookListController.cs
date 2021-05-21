using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SingularitybdWeb.ApiModels;
using SingularitybdWeb.Controllers.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SingularitybdWeb.Controllers
{
    public class BookListController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        //private readonly DownDbTestContext DbTestContext;
        // BookListController(DownDbTestContext downDb)
        // {
        //     DbTestContext = downDb;
        // }
        //GET: BookListController
        [Authorize("Authorization")]
        public async Task<ActionResult> Index()
        {
            if (JwtMethods.IsExpired())
            {
                var result = await JwtMethods.RefreshToken(User);
                MyToken.expiration = result.expiration;
                MyToken.token = result.token;
            }
            httpClient.BaseAddress = MyToken.BaseUrl;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
            List<BookList> list= await httpClient.GetFromJsonAsync<List<BookList>>("BookLists");
            return View(list);
        }

        

        // GET: BookListController/Create
        [Authorize("Authorization")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BookListController/Create
        [Authorize("Authorization")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookList model)
        {
            if (ModelState.IsValid)
            {
                model.Udate = DateTime.Now;
                try
                {
                    if (JwtMethods.IsExpired())
                    {
                        var result = await JwtMethods.RefreshToken(User);
                        MyToken.expiration = result.expiration;
                        MyToken.token = result.token;
                    }
                    httpClient.BaseAddress = MyToken.BaseUrl;

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
                    var res = await httpClient.PostAsJsonAsync<BookList>("BookLists",model);
                    if (res.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                        return View();
                   
                }
                catch
                {
                    return View();
                }
            }
            return View();
        }

        // GET: BookListController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                if (JwtMethods.IsExpired())
                {
                    var result = await JwtMethods.RefreshToken(User);
                    MyToken.expiration = result.expiration;
                    MyToken.token = result.token;
                }
                httpClient.BaseAddress = MyToken.BaseUrl;

                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
                BookList book = await httpClient.GetFromJsonAsync<BookList>("BookLists/"+id);
                if (book!=null)
                {
                    return View(book);
                }
                else
                    return RedirectToAction(nameof(Index));

            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            
        }

        // POST: BookListController/Edit/5
        [Authorize("Authorization")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, BookList book)
        {
            try
            {
                book.Udate = DateTime.Now;
                if (JwtMethods.IsExpired())
                {
                    var result = await JwtMethods.RefreshToken(User);
                    MyToken.expiration = result.expiration;
                    MyToken.token = result.token;
                }
                httpClient.BaseAddress = MyToken.BaseUrl;

                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
                var res = await httpClient.PutAsJsonAsync<BookList>("BookLists/"+id, book);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View();
            }
            catch
            {
                return View();
            }
        }


        // GET: BookListController/Delete/5
        [Authorize("Authorization")]
        public async Task<ActionResult> Delete(int id)
        {
            
            if (JwtMethods.IsExpired())
            {
                var result = await JwtMethods.RefreshToken(User);
                MyToken.expiration = result.expiration;
                MyToken.token = result.token;
            }
            httpClient.BaseAddress = MyToken.BaseUrl;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
            var res = await httpClient.DeleteAsync("BookLists/" + id);
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            return RedirectToAction(nameof(Index));
        }

      
    }
}
