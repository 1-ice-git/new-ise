using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.DBModel
{
    public class AttivazioneMABModel
    {
        [Key]
        public decimal idAttivazioneMAB { get; set; }

        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        public decimal? idTrasferimento { get; set; }

        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Notifica rich.")]
        public bool notificaRichiesta { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data not. rich.")]
        public DateTime? dataNotificaRichiesta { get; set; }

        [Required()]
        [DefaultValue(false)]
        [Display(Name = "Attivaz. rich.")]
        public bool Attivazione { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd/mm/yyyy}")]
        [Display(Name = "Data attivazione")]
        public DateTime? dataAttivazione { get; set; }

        [Required()]
        [DefaultValue(false)]
        public bool Annullato { get; set; }

        public bool HasValue()
        {
            return idAttivazioneMAB > 0 ? true : false;
        }
        public DateTime? dataAggiornamento { get; set; }
        public DateTime? dataVariazione { get; set; }

    }
}