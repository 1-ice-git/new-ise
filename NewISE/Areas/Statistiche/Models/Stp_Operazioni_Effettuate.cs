using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Operazioni_Effettuate
    {
        [Key]
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        public string nominativo { get; set; }
        public string sede { get; set; }
        public string valuta { get; set; }
        public string tipomovimento { get; set; }
        public string dataDecorrenza { get; set; }
        public string dataLettera { get; set; }
        public string importo1 { get; set; }
        public string importo2 { get; set; }
        public string importo3 { get; set; }
        public string dataOperazione { get; set; }
        public string codLivello { get; set; }
        public string tipoRecord { get; set; }
        public string tipoSpesa { get; set; }
        public string utente { get; set; }

    }
}