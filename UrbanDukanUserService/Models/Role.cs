using System;
using System.Collections.Generic;

namespace UrbanDukanUserService.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
