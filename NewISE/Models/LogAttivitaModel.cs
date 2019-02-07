
using NewISE.Models.DBModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{


    public class LogAttivitaModel
    {
        [Key]
        public decimal idLog { get; set; }

        [Required(ErrorMessage = "Id Utente Loggato richiesto")]
        public decimal idDipendente { get; set; }

        public decimal? idTrasferimento { get; set; } = null;

        [Required(ErrorMessage = "Id Attivita Crud richiesto")]
        public decimal idAttivitaCrud { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Required(ErrorMessage = "Descrizione Attività Svolta")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Attività svolta")]
        public string descAttivitaSvolta { get; set; }

        [StringLength(60, ErrorMessage = "Il campo accetta un massimo di 60 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Tabella coinvolta", Name = "Tabella coinvolta")]
        public string tabellaCoinvolta { get; set; } = null;

        public decimal? idTabellaCoinvolta { get; set; } = null;

        public UtenteAutorizzatoModel utenteAutorizzato { get; set; }

        public AttivitaCRUDModel attivitaCrud { get; set; }

        public TrasferimentoModel trasferimento { get; set; }
    }
}