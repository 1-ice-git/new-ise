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

        [Display(Name = "Data Inizio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizioValidita { get; set; }

        [Display(Name = "Data Fine")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataFineValidita { get; set; }

        [Display(Name = "Canone")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal ImportoCanone { get; set; }


        public DateTime DataAggiornamento { get; set; }

        public decimal idStatoRecord { get; set; }

        public decimal? FK_IDCanone { get; set; }

        public bool HasValue()
        {
            return idCanone > 0 ? true : false;
        }

        public TFRModel TFR;

        public decimal idValuta { get; set; }

    }
}