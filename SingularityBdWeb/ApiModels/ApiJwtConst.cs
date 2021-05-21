using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularitybdWeb.ApiModels
{
    public class ApiJwtConst
    {
        public const string Issuer = "Hridoy";
        public const string Audience = "ApiUser";
        public const string key = "27381243821648129193239";

        public const string AuthSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
