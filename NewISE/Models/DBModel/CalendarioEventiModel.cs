using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{

    public enum EnumFunzioniEventi
    {
        RichiestaMaggiorazioniFamiliari = 1,
        RichiestePratichePassaporto = 2,
        RichiestaTitoliViaggio = 3,
        RichiestaTrasportoEffettiPartenza = 21,
        RichiestaTrasportoEffettiRientro = 22,
        RichiestaAnticipi = 23,
        RichiestaMaggiorazioneAbitazione = 24,
        AttivaTrasferimento = 25,
        InvioPassaporto = 26,
        RichiestaProvvidenzeScolastiche = 27
    }
    public class CalendarioEventiModel
    {
        [Key]
        public decimal idCalendarioEventi { get; set; }
        [Required]
        public EnumFunzioniEventi idFunzioneEventi { get; set; }
        public decimal idTrasferimento { get; set; }
        [Required(ErrorMessage = "La data inizio evento è richiesta.")]

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInizioEvento { get; set; }

        [Required(ErrorMessage = "La data scadenza è richiesta.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataScadenza { get; set; }
        public bool Completato { get; set; }
        [Required(ErrorMessage = "La data completato è richiesta.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataCompletato { get; set; }
        public bool Annullato { get; set; }
    }

}