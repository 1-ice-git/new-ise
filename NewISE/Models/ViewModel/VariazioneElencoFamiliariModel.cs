using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.DBModel.Enum;

namespace NewISE.Models.DBModel
{
    public class VariazioneElencoFamiliariModel
    {
        public decimal idMaggiorazioniFamiliari { get; set; }

        public decimal idAttivazioneMagFam { get; set; }

        public decimal idFamiliare { get; set; }///ID del figlio o del coniuge

        public string Nominativo { get; set; }

        [Display(Name = "Cod. Fisc.")]
        public string CodiceFiscale { get; set; }

        [Display(Name = "Data Ini.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataInizio { get; set; }

        [Display(Name = "Data Fin.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime? dataFine { get; set; }

        [Display(Name = "Parentela")]
        public EnumParentela parentela { get; set; }

        [Display(Name = "Pensione")]
        public bool HasPensione { get; set; }

        [Display(Name = "Altri dati")]
        public decimal idAltriDati { get; set; }

        [Display(Name = "Escludi P.")]
        [DefaultValue(false)]
        public bool escludiPassaporto { get; set; }

        [Display(Name = "Personal.")]
        [DefaultValue(false)]
        public bool personalmente { get; set; }

        [Display(Name = "Escludi t.v.")]
        [DefaultValue(false)]
        public bool escludiTitoloViaggio { get; set; }

        [Display(Name = "Documenti")]
        public IList<VariazioneDocumentiModel> Documenti { get; set; }

        public bool eliminabile { get; set; }

        public bool modificabile { get; set; }

        public bool pensione_modificata { get; set; }

        public bool adf_modificati { get; set; }

        public bool doc_modificati { get; set; }

        [Display(Name = "Stato")]
        public decimal idStatoRecord { get; set; }

    }
}