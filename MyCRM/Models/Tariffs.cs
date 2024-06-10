using System;
using System.Collections.Generic;

namespace MyCRM.Models
{
    public partial class Tariffs
    {
        public Tariffs()
        {
            Users = new HashSet<Users>();
        }

        public int TId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
