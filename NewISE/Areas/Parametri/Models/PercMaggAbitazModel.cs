using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Areas.Parametri.Models
{
    public class PercMaggAbitazModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idPercMabAbitaz { get; set; }
        public decimal idUfficio { get; set; }
        public decimal idLivello { get; set; }
        public DateTime dataInizioValidita { get; set; }
        public DateTime? dataFineValidita { get; set; }
        public decimal percentuale { get; set; }
        public bool annullato { get; set; } = false;

        public LivelloModel Livello { get; set; }
        public UfficiModel DescrizioneUfficio { get; set; }

    }
}