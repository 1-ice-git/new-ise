using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ElencoVociManualiViewModel
    {
        [Key]
        public decimal idAutoVociManuali { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Ufficio")]
        public string ufficio { get; set; }
        [Display(Name = "Voce")]
        public string voce { get; set; }
        [Display(Name = "Da Mese/Anno")]
        public string meseAnnoInizio { get; set; }
        [Display(Name = "A Mese/Anno")]
        public string meseAnnoFine { get; set; }
        [Display(Name = "Importo")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal importo { get; set; }

    }
}