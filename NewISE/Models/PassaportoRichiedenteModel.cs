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
        public decimal idPassaporti { get; set; }
        [Required(ErrorMessage = "Escludi passaporto è richiesto.")]
        [Display(Name = "Escludi passaporto")]
        [DefaultValue(false)]
        public bool EscludiPassaporto { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data escl. pass.")]
        public DateTime DataEscludiPassaporto { get; set; }
        [Display(Name = "Data agg.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataAggiornamento { get; set; }
        [Required(ErrorMessage = "Annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }


        public PassaportoModel Passaporto { get; set; }

    }
}