using System;
using System.Collections.Generic;

#nullable disable

namespace Project.Model.Core
{
    public partial class User
    {
        public User()
        {
            News = new HashSet<News>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
