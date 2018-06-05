using NewISE.Models.DBModel;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public struct HasDoc
    {
        public EnumTipoDoc tipoDoc { get; set; }
        public bool esisteDoc { get; set; }
    }
    public struct HasDocPassaporto
    {
        public EnumTipoDoc tipoDocPassaporto { get; set; }
        public bool esisteDocPassaporto { get; set; }
        public decimal idDocPassaporto { get; set; }
    }

    public class ElencoFamiliariPassaportoModel
    {
        public decimal idAttivazionePassaporti { get; set; }

        public decimal idAttivazioneInvioPassaporti { get; set; }

        public decimal idFamiliarePassaporto { get; set; }

        public decimal idFamiliareInvioPassaporto { get; set; }
        [Display(Name = "Nominativo")]
        public string nominativo { get; set; }
        [Display(Name = "Cod. Fisc.")]
        public string codiceFiscale { get; set; }
        [Display(Name = "Dt. Ini.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dataInizio { get; set; }
        [Display(Name = "Dt. fin.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dataFine { get; set; }
        [Display(Name = "Parentela")]
        public EnumParentela parentela { get; set; }
        [Display(Name = "A. d. Fam.")]
        public decimal idAltriDati { get; set; }
        [Display(Name = "Richiedi")]
        [DefaultValue(false)]
        public bool richiedi { get; set; }

        public bool notificato { get; set; }
        public bool attivato { get; set; }

        public HasDoc HasDoc { get; set; }
        public HasDocPassaporto HasDocPassaporto { get; set; }

        public decimal ordinamento { get; set; }

    }
}