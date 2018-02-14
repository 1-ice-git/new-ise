using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class CanoneMABModel
    {
        [Key]
        [Display(Name = "ID")]
        public decimal idCanone { get; set; }

        public decimal IDAttivazioneMAB { get; set; }

        public decimal IDMAB { get; set; }

        public DateTime DataInizioValidita { get; set; }

        public DateTime DataFineValidita { get; set; }

        [Display(Name = "Canone")]
        public decimal ImportoCanone { get; set; }

        public DateTime DataAggiornamento { get; set; }

        public bool Annullato { get; set; }

        public bool HasValue()
        {
            return idCanone > 0 ? true : false;
        }

        public TFRModel TFR;

    }
}