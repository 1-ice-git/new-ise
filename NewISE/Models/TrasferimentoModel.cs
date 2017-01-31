using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models
{
    public class TrasferimentoModel
    {
        [Key]
        public long idTrasferimento { get; set; }
        [Required(ErrorMessage = "ID Trasferimento richiesto")]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]

        public long idTipoTrasferimento { get; set; }
        [Required(ErrorMessage = "ID TipoTrasferimento richiesto")]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]

        public long idUfficio { get; set; }
        [Required(ErrorMessage = "ID Ufficio richiesto")]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]

        public long idStatoTrasferimento{ get; set; }
        [Required(ErrorMessage = "ID StatoTrasferimento richiesto")]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]

        public long idRuolo { get; set; }
        [Required(ErrorMessage = "ID Ruolo")]
        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]

        public DateTime dataPartenza { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di partenza")]

        public DateTime dataRientro { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di rientro")]

        public string coan { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "COAN")]

        public string protocolloLettera { get; set; }
        [Required]
        [StringLength(10)]
        [Display(Name = "Protocollo Lettera")]

        public DateTime dataLettera { get; set; }
        public int annullato { get; set; }
        public LogAttivitaModel logAttivita { get; set; }

        public string Trasferimento
        {
            get
            {
                return dataPartenza + ", " + dataRientro + ", " + protocolloLettera;
            }
        }

        public virtual LogAttivitaModel LOGATTIVITA { get; set; }

    }
}