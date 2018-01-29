using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class UfficiModel
    {
        [Key]
        public decimal idUfficio { get; set; }

        [Required(ErrorMessage = "Il codice ufficio è richiesto.")]
        [StringLength(30, ErrorMessage = "Per il codice ufficio sono ammessi massimo 4 caratteri.")]
        [Display(Name = "CodiceUfficio")]
        [DataType(DataType.Text)]
        public string codiceUfficio { get; set; }
        [Required(ErrorMessage = "La descrizione ufficio è richiesta.")]
        [StringLength(30, ErrorMessage = "Per la descrizione dell'ufficio sono ammessi massimo 50 caratteri.")]
        [Display(Name = "Descrizione")]
        public string descUfficio { get; set; }
        [DefaultValue(false)]
        [Display(Name = "Pag. val. uff.")]
        public bool pagatoValutaUfficio { get; set; }

        public IList<ValuteModel> lValutaUfficio { get; set; }


    }
}