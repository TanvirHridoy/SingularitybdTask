using System;
using System.Collections.Generic;

namespace SingularitybdWeb.ApiModels
{
    public partial class Files
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public bool? IsDeleted { get; set; }
        public string OwnerUserId { get; set; }
        public DateTime? DeleteDate { get; set; }

        public virtual AspNetUsers OwnerUser { get; set; }
    }
}
