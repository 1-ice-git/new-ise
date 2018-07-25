using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class MeseAnnoElaborazioneModel
    {
        [Key]
        public decimal idMeseAnnoElab { get; set; }
        [Display(Name = "Mese")]
        [DisplayFormat(DataFormatString = "{0:00}")]
        public int mese { get; set; }
        [Display(Name = "Anno")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int anno { get; set; }
        [Display(Name = "Chiuso")]
        public bool chiuso { get; set; }
    }
}