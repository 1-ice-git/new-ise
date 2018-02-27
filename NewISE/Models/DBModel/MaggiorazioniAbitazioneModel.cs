using NewISE.Models.Tools;
using System;
using NewISE.Models.DBModel.dtObj;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{


    public class MaggiorazioneAbitazioneModel
    {
        [Key]
        public decimal idMAB { get; set; }

        public decimal idTrasferimento { get; set; }
        public decimal idAttivazioneMAB { get; set; }
        public DateTime dataPartenza { get; set; }

        [Display(Name = "Inizio Validità")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [CustomValidation(typeof(dtMaggiorazioneAbitazione), "VerificaDataInizio", ErrorMessage = "La data inizio maggiorazione abitazione non può essere inferiore alla data di trasferimento.")]
        public DateTime dataInizioMAB { get; set; }
        [Display(Name = "Fine Validità")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dataFineMAB { get; set; } = Convert.ToDateTime(Utility.DataFineStop());

        [Display(Name = "Tipologia Anticipo")]
        public bool AnticipoAnnuale { get; set; }

        public DateTime dataAggiornamento { get; set; }

        public bool Annullato { get; set; }

        public bool HasValue()
        {
            return idMAB > 0 ? true : false;
        }

        public CanoneMABModel CanoneMAB;
    }
}