using System;
using System.Collections.Generic;

#nullable disable

namespace Project.Model.Core
{
    public partial class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
