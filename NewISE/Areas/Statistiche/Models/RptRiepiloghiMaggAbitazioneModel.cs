using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Statistiche.Models
{
    public class RptRiepiloghiMaggAbitazioneModel
    {
        public string Matricola { get; set; }
        public string Nominativo { get; set; }
        public string Ufficio { get; set; }
        public string MeseRiferimento { get; set; }
        public string MeseElaborazione { get; set; }
        public string Valuta { get; set; }
        public decimal percApplicata { get; set; }
        public decimal Canone { get; set; }
        public decimal importo_mab { get; set; }
        public decimal importo_ind_pers { get; set; }
        public decimal numMeseRiferimento { get; set; }
        public decimal numMeseElaborazione { get; set; }
        public decimal tfr { get; set; }
    }
}