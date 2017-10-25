using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class RichiedenteMagFamViewModel : ElencoFamiliariViewModel
    {
        [Key]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal idTrasferimento { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool richiestaAttivazione { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool attivaMaggiorazioni { get; set; }
        [Required]
        public DateTime dataAggiornamento { get; set; }

    }
}