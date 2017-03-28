using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NewISE.Models
{
    public class TrasferimentoModel
    {
        [Key]        
        public long idTrasferimento { get; set; }

        [Required(ErrorMessage = "Tipo trasferimento richiesto")]        
        public long idTipoTrasferimento { get; set; }

        [Required(ErrorMessage = "Ufficio richiesto")]
        public long idUfficio { get; set; }

        [Required(ErrorMessage = "Stato trasferimento richiesto")]
        public long idStatoTrasferimento { get; set; }

        [Required(ErrorMessage = "Ruolo richiesto.")]
        public long idRuolo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di partenza")]
        [Required(ErrorMessage = "la data di partenza è richiesta.")]
        public DateTime dataPartenza { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di rientro")]
        public DateTime? dataRientro { get; set; }

        
        [StringLength(10, ErrorMessage = "per ilo COAN sono richiesti un massimo di 10 caratteri.")]
        [Display(Name = "COAN")]
        [DataType(DataType.Text)]
        public string coan { get; set; }

        
        [StringLength(100, ErrorMessage = "per il protocollo lettera sono richiesti un massimo di 100 caratteri.")]
        [Display(Name = "Protocollo Lettera")]
        [DataType(DataType.Text)]
        public string protocolloLettera { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime? dataLettera { get; set; }

        [Required(ErrorMessage ="Il flag annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }
       

        

    }
}
