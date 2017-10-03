using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Dislocazione_dipendenti
    {
        [Key]
        public string sede { get; set; }
        public string valuta { get; set; }
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string dataTrasferimento { get; set; }
        public string qualifica { get; set; }
        public string coniuge { get; set; }
        public string figli { get; set; }
        public string isep { get; set; }
        public string contributo { get; set; }
        public string uso { get; set; }
        public string totale { get; set; }

    }
}