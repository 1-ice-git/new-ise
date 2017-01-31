using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NewISE.Models
{
    public class LogAttivitaModel
    {
        [Key]
        [Required(ErrorMessage = "Id Log richiesto")]
        public long idLog { get; set; }

        [Required(ErrorMessage = "Id Utente Loggato richiesto")]
        public long idUtenteLoggato { get; set; }

        [Required(ErrorMessage = "Id Trasferimento richiesto")]
        public long idTrasferimento { get; set; }

        [Required(ErrorMessage = "Id Attivita Crud richiesto")]
        public long idAttivitaCrud { get; set; }

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
        public string tabellaCoinvolta { get; set; }
        

        public long idTabellaCoinvolta { get; set; }
        [Required(ErrorMessage = "idTabellaCoinvolta")]

        public AttivitaCRUDModel attivitaCrudM { get; set; }

        public virtual TrasferimentoModel TRASFERIMENTO { get; set; }
        
        public string LogAttivitaIse
        {
            get
            {
                return idLog + ", " + descAttivitaSvolta;
            }
        }

    }
}