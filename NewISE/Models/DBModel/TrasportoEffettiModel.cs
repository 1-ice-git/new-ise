using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TrasportoEffettiModel
    {
        [Key]
        public decimal idTrasportoEffetti { get; set; }
        [Required(ErrorMessage = "Il tipo di trasporto è richiesto.")]
        [Display(Name = "Tipo trasf.")]
        public decimal idTipoTrasporto { get; set; }
        [Required(ErrorMessage = "Il trasferimento è richiesto.")]
        [Display(Name = "Trasferimento")]
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Data agg.")]
        public DateTime dataAggiornamento { get; set; }
        [Required(ErrorMessage = "Il campo annullato è richiesto.")]
        [DefaultValue(false)]
        [Display(Name = "Annullato")]
        public bool annullato { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }
        public TipoTrasportoModel TipoTrasporto { get; set; }

    }
}