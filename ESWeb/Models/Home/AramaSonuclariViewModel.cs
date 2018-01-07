using Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESWeb.Models.Home
{
    public class AramaSonuclariViewModel
    {
        public List<Urun> UrunListesi { get; set; }
        public string HataMesaji { get; set; }
    }
}