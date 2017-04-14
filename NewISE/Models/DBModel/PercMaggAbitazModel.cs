using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
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

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }

        public bool annullato { get; set; } = false;

        public LivelloModel Livello { get; set; }
        public UfficiModel DescrizioneUfficio { get; set; }

    }
}