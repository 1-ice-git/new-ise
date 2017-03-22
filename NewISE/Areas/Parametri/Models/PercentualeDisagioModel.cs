using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace NewISE.Areas.Parametri.Models
{
    public class PercentualeDisagioModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercentualeDisagio { get; set; }
        public decimal? idUfficio { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentuale { get; set; }
        public bool annullato { get; set; } = false;
        public UfficiModel DescrizioneUfficio { get; set; }
        


    }
}