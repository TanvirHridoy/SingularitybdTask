using System;
using System.Collections.Generic;

#nullable disable

namespace SingularityApi.Models
{
    public partial class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public bool? IsDeleted { get; set; }
        public string OwnerUserId { get; set; }
        public DateTime? DeleteDate { get; set; }

        public virtual AspNetUser OwnerUser { get; set; }
    }
}
