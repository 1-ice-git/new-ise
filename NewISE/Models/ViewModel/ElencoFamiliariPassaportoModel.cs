using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ElencoFamiliariPassaportoModel
    {
        public decimal idFamiliarePassaporto { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Cod. Fisc.")]
        public string codiceFiscale { get; set; }
        [Display(Name = "Dt. Ini.")]
        public DateTime dataInizio { get; set; }
        [Display(Name = "Dt. fin.")]
        public DateTime? dataFine { get; set; }
        [Display(Name = "Parentela")]
        public EnumParentela parentela { get; set; }
        [Display(Name = "Altri dati Fam.")]
        public decimal idAltriDati { get; set; }
        [Display(Name = "Richiedi")]
        [DefaultValue(false)]
        public bool richiedi { get; set; }

        public bool HasDoc { get; set; }
    }
}