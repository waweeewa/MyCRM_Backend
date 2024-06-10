using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.DAL.DataModel
{
    public partial class Korisnici
    {
        public int ID_korisnika { get; set; }
        public string email { get; set; }
        public string lozinka { get; set; }
    }
}
