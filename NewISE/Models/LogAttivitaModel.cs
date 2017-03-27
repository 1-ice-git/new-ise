using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace NewISE.Models
{
    public class LogAttivitaModel
    {
        [Key]
        [Required(ErrorMessage = "Id Log richiesto")]
        public decimal idLog { get; set; }

        [Required(ErrorMessage = "Id Utente Loggato richiesto")]
        public decimal idUtenteLoggato { get; set; }

        [Required(ErrorMessage = "Id Trasferimento richiesto")]
        public decimal? idTrasferimento { get; set; } = null;

        [Required(ErrorMessage = "Id Attivita Crud richiesto")]
        public decimal idAttivitaCrud { get; set; }

        [Required(ErrorMessage = "Descrizione utente richiesta")]
        [StringLength(30, ErrorMessage = "Il campo accetta un massimo di 30 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Descrizione dell'utente", Name = "Utente")]
        public string utente { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Required(ErrorMessage = "Descrizione Attività Svolta")]
        public string descAttivitaSvolta { get; set; }

        [Required(ErrorMessage = "Tabella coinvolta")]
        [StringLength(60, ErrorMessage = "Il campo accetta un massimo di 60 caratteri.")]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true, Description = "Tabella coinvolta", Name = "Tabella coinvolta")]
        public string tabellaCoinvolta { get; set; } = null;

        public decimal? idTabellaCoinvolta { get; set; } = null;

        [Required(ErrorMessage = "idTabellaCoinvolta")]

       // public AttivitaCRUDModel attivitaCrudM { get; set; }

        //public virtual TrasferimentoModel TRASFERIMENTO { get; set; }

        public string LogAttivitaIse
        {
            get
            {
                return idLog + ", " + descAttivitaSvolta;
            }
        }

    }
}