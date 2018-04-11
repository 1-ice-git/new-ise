using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AttivazioniProvScolasticheModel
    {
        [Key]
        public decimal idProvScolastiche { get; set; }

        [Required(ErrorMessage = "idTrasfProvScolastiche è richiesto.")]
        [Display(Name = "idTrasfProvScolastiche")]
        public decimal idTrasfProvScolastiche { get; set; }

        [Required(ErrorMessage = "Il valore per la Notifica Richiesta è obbligatorio.")]
        [DefaultValue(false)]
        [Display(Name = "Notifica Richiesta")]
        public bool notificaRichiesta { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Notifica")]
        public DateTime? dataNotifica { get; set; }

        [Required(ErrorMessage = "Il valore per l'attivazione della richiesta è obbligatorio.")]
        [DefaultValue(false)]
        [Display(Name = "Attiva Richiesta")]
        public bool attivaRichiesta { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Attivazione")]
        public DateTime? dataAttivazione { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; } = DateTime.Now;

        [Required]
        [DefaultValue(false)]
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }

        public ProvvidenzeScolasticheModel ProvvidenzeScolastiche { get; set; }

    }
}