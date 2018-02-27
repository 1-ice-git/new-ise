using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Contributo_Abitazione
    {
        [DisplayName("Matricola")]
        public string matricola { get; set; }
        [DisplayName("Nominativo")]
        public string nominativo { get; set; }
        [DisplayName("Sede")]
        public string sede { get; set; }
        [DisplayName("Valuta")]
        public string valuta { get; set; }
        [DisplayName("Data Decorrenza")]
        public string data_decorrenza { get; set; }
        [DisplayName("Data Lettera")]
        public string data_lettera { get; set; }
        [DisplayName("Data Operazione")]
        public string data_operazione { get; set; }
        [DisplayName("Contributo Valuta")]
        public string contributo_valuta { get; set; }
        [DisplayName("Contributo L/E")]
        public string contributo_L_E { get; set; }
        [DisplayName("Canone")]
        public string canone { get; set; }
        [DisplayName("% applicata")]
        public string percentuale_applicata { get; set; }

    }
}