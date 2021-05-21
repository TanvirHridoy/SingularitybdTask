using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularityApi.Models
{
    public class Token
    {
       
            public  string token { get; set; }
            public  DateTime expiration { get; set; }
    }
}
