using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class AttivazionePassaportiModel
    {
        [Key]
        public decimal idAttivazioniPassaporti { get; set; }
        [Required]
        [Display(Name = "Passaporto")]
        public decimal idPassaporti { get; set; }
        [Required(ErrorMessage = "Il campo notifica richiesta è richiesto.")]
        [DefaultValue(false)]
        [Display(Name = "Notifica rich.")]
        public bool notificaRichiesta { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data notifica")]
        public DateTime? dataNotificaRichiesta { get; set; }
        [Required(ErrorMessage = "OIl campo pratica conclusa è richiesto.")]
        [DefaultValue(false)]
        [Display(Name = "Pratica concl.")]
        public bool praticaConclusa { get; set; }
        [Required(ErrorMessage = "Il campo data pratica conclusa è richiesto.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Att.")]
        public DateTime? dataPraticaConclusa { get; set; }

        //[Required(ErrorMessage = "Il campo Escludi passaporto è richiesto.")]
        //[Display(Name = "Escludi P.")]
        //[DefaultValue(false)]
        //public bool escludiPassaporto { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data Agg.")]
        public DateTime dataAggiornamento { get; set; } = DateTime.Now;
        [Required]
        [DefaultValue(false)]
        [Display(Name = "Annullato")]
        public bool annullato { get; set; } = false;

        public PassaportoModel Passaporto { get; set; }

        public bool HasValue()
        {
            return this.idAttivazioniPassaporti > 0 ? true : false;
        }
    }
}