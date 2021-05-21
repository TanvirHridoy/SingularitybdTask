using SingularitybdWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularitybdWeb.Models
{
    public class MenuViewModel
    {
        public List<NavigationMenu> menus = new List<NavigationMenu>();
        public string Status { get; set; }
    }
}
