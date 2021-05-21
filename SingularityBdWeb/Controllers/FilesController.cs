using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SingularitybdWeb.ApiModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SingularitybdWeb.Controllers
{
    public class FilesController : Controller
    {
        private readonly HttpClient httpClient = new HttpClient();
        // GET: FilesController
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
            var list = await httpClient.GetFromJsonAsync<List<Files>>("Files");
            //List<Files>
            return View(list);
        }



        [Authorize("Authorization")]
        // GET: FilesController/Create
        public ActionResult Create()
        {

            return View();
        }

        [Authorize("Authorization")]
        // POST: FilesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Files model)
        {
            if (ModelState.IsValid)
            {
                model.IsDeleted = false;

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
                    var res = await httpClient.PostAsJsonAsync<Files>("Files", model);
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

        // GET: FilesController/Edit/5
        [Authorize("Authorization")]
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
                Files file = await httpClient.GetFromJsonAsync<Files>("Files/" + id);
                if (file != null)
                {
                    return View(file);
                }
                else
                    return RedirectToAction(nameof(Index));

            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }


        }

        // POST: FilesController/Edit/5
        [Authorize("Authorization")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Files file)
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
                var res = await httpClient.PutAsJsonAsync<Files>("Files/" + id, file);
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

        // GET: FilesController/Delete/5
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
            var res = await httpClient.DeleteAsync("Files/" + id);
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
                return RedirectToAction(nameof(Index));
        }

        [Authorize("Authorization")]
        public async Task<ActionResult> Trash()
        {
            if (JwtMethods.IsExpired())
            {
                var result = await JwtMethods.RefreshToken(User);
                MyToken.expiration = result.expiration;
                MyToken.token = result.token;
            }
            httpClient.BaseAddress = MyToken.BaseUrl;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
            var list = await httpClient.GetFromJsonAsync<List<Files>>("Trash");
            //List<Files>
            return View(list);

        }

        [Authorize("Authorization")]
        public async Task<ActionResult> DeleteTrash(int id)
        {
            if (JwtMethods.IsExpired())
            {
                var result = await JwtMethods.RefreshToken(User);
                MyToken.expiration = result.expiration;
                MyToken.token = result.token;
            }
            httpClient.BaseAddress = MyToken.BaseUrl;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + MyToken.token);
            var res = await httpClient.DeleteAsync("Trash/" + id);
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Trash));
            }
            else
                return RedirectToAction(nameof(Trash));
        }

        [Authorize("Authorization")]
        public async Task<IActionResult> RestoreTrash(int id)
        {
            Files file = new Files();
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
                var res = await httpClient.PutAsJsonAsync<Files>("Trash/" + id, file);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Trash));
            }
            catch
            {
                return RedirectToAction(nameof(Trash));
            }
        }


    }
}
