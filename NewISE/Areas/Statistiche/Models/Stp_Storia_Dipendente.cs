using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewISE.Areas.Statistiche.Models
{
    public class Stp_Storia_Dipendente
    {
        
        public string NOMINATIVO { get; set; }
        public string MATRICOLA { get; set; }
        public string LIVELLO { get; set; }
        public string TIPO_MOVIMENTO { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Decorrenza")]
        public string DATA_DECORRENZA { get; set; }
        public string DATA_LETTERA { get; set; }
        public string COEF_SEDE { get; set; }
        public string PERC_DISAGIO { get; set; }
        public string PERC_SPETTANTE { get; set; }
        public string PERC_CONIUGE { get; set; }
        public string PENSIONE { get; set; }
        public string N_FIGLI { get; set; }
        public string TFR { get; set; }
        public string DESCR { get; set; }
        public string INDENNITA { get; set; }
        public string ORD1 { get; set; }
        public string ORD2 { get; set; }
        public string ORD3 { get; set; }
        public List<SelectListItem> lstCategory { get; set; }
        public Int16 selected { get; set; }
        
        public string Key { get; set; }
        public string Display { get; set; }
    }
    

}