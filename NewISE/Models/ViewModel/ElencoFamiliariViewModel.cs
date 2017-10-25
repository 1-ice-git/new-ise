using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel;

namespace NewISE.Models.ViewModel
{
    public abstract class ElencoFamiliariViewModel
    {

        public virtual string Nominativo { get; set; }
        [Display(Name = "Cod. Fisc.")]
        public virtual string CodiceFiscale { get; set; }
        [Display(Name = "Data Ini.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataInizio { get; set; }
        [Display(Name = "Data Fin.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataFine { get; set; }
        [Display(Name = "Parentela")]
        public virtual EnumParentela parentela { get; set; }

        [Display(Name = "Altri dati")]
        public virtual decimal idAltriDati { get; set; }

        [Display(Name = "Escludi P.")]
        [DefaultValue(false)]
        public virtual bool escludiPassaporto { get; set; }

        [Display(Name = "Escludi t.v.")]
        [DefaultValue(false)]
        public virtual bool escludiTitoloViaggio { get; set; }

        [Display(Name = "Documenti")]
        public virtual IList<DocumentiModel> Documenti { get; set; }
    }
}