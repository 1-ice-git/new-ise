using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Dipendenti
    {
        public List<SelectListItem> Dipendenti { get; set; }
        public string matricola { get; set; }
        public string nominativo { get; set; }

        public Int16 selected { get; set; }



       
    }
}