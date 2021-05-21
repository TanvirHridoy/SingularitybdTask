using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SingularitybdWeb.Models;
using System.Net.Http.Json;

namespace SingularitybdWeb.ApiModels
{
    public static class MyToken
    {
        public static Uri  BaseUrl = new Uri("http://localhost:17250/api/");

        public static string token { get; set; }
        public static DateTime expiration { get; set; }



    }
    public  class JwtToken
    {
        public  string token { get; set; }
        public  DateTime expiration { get; set; }



    }
    public static class JwtMethods
    {
        public static async  Task<JwtToken> CheckJwtValidity(string Username,string password)
        {
            
                HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = MyToken.BaseUrl;
                
                var res = await httpClient.PostAsJsonAsync<JwtTokenViewModel>("account", new JwtTokenViewModel { UserName = Username, Password = password });
                res.EnsureSuccessStatusCode();
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<JwtToken>();
            }

            else return new JwtToken();
            //return res;
        }

        public static async Task<JwtToken> RefreshToken(ClaimsPrincipal claim)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = MyToken.BaseUrl;
            var res = await httpClient.PostAsJsonAsync<string>("account/"+claim.Identity.Name,"");
            res.EnsureSuccessStatusCode();
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<JwtToken>();
            }

            else return new JwtToken();
        }

        public static bool IsExpired()
        {
            bool result = MyToken.expiration < DateTime.UtcNow ? true : false;
            return result;
        }
    }
}
