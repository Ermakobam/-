using System;
using System.Collections.Generic;

namespace Shop.Core.Entities
{
    public class User : BaseEntity
    {
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
