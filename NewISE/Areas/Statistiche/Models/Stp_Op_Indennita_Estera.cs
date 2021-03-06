﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Op_Indennita_Estera
    {
        [Display(Name = "Matricola")]
        public string matricola { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Qualifica")]
        public string qualifica { get; set; }
        [Display(Name = "Sede")]
        public string sede { get; set; }
        [Display(Name = "Valuta")]
        public string valuta { get; set; }
        [Display(Name = "Codice Tipo Movimento")]
        public string codice_tipo_movimento { get; set; }
        [Display(Name = "Tipo Movimento")]
        public string tipo_movimento { get; set; }
        [Display(Name = "Data Decorrenza")]
        public string data_decorrenza { get; set; }
        [Display(Name = "Data Lettera")]
        public string data_lettera { get; set; }
        [Display(Name = "Data Operazione")]
        public string data_operazione { get; set; }
        [Display(Name = "Indennità Personale")]
        public string indennita_personale { get; set; }
        [Display(Name = "Sist. /Rientro Lorda")]
        public string sist_rientro_lorda { get; set; }
        [Display(Name = "Anticipo")]
        public string anticipo { get; set; }
        
    }
}