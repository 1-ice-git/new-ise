using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models
{
    public class PassaportoRichiedenteModel
    {
        [Key]
        public decimal idPassaportoRichiedente { get; set; }
        [Required(ErrorMessage = "Il passaporto è richiesto.")]
        [Display(Name = "Passaporto")]
        public decimal idPassaporto { get; set; }
        [Required(ErrorMessage = "L'attivazione del passaporto è richiesta.")]
        [Display(Name = "Att. Passaporto")]
        public decimal idAttivazionePassaporti { get; set; }
        [Required(ErrorMessage = "Escludi passaporto è richiesto.")]
        [Display(Name = "Escludi passaporto")]
        [DefaultValue(false)]
        public bool includiPassaporto { get; set; }
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dataAggiornamento { get; set; }
        [Required(ErrorMessage = "Annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }


        public PassaportoModel Passaporto { get; set; }

    }
}