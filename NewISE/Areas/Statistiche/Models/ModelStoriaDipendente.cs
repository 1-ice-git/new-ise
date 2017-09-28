using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Models
{
    public class ModelStoriaDipendente
    {
        public Stp_Storia_Dipendente Stp_Storia_Dipendente { get; set; }
        public CategoryModel CategoryModel { get; set; }
    }
}