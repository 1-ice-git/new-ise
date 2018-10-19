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
        public string DataPartenza { get; set; }
        public string DataLettera { get; set; }
        public string DataOperazione { get; set; }
        public decimal percApplicata { get; set; }
        public decimal Canone { get; set; }
        public decimal Importo { get; set; }
    }
}