using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingularitybdWeb.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    

    class ItemEqualityComparer : IEqualityComparer<NavigationMenuViewModel>
    {
        public bool Equals(NavigationMenuViewModel x, NavigationMenuViewModel y)
        {
            // Two items are equal if their keys are equal.
            return x.Id == y.Id;
        }

        public int GetHashCode(NavigationMenuViewModel obj)
        {
            return obj.Id.GetHashCode();
        }
    }

}
