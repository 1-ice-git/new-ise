﻿using NewISE.Models.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.ViewModel
{
    public class ElencoFamiliariModel
    {
        public decimal id { get; set; }///Può essere sia l'id delle maggiorazioni figlio sia l'id delle maggiorazioni coniuge.
        public decimal idTrasferimento { get; set; }
        public decimal idFamiliare { get; set; }///ID del figlio o del coniuge
        public string Nominativo { get; set; }
        [Display(Name = "Cod. Fisc.")]
        public string CodiceFiscale { get; set; }
        [Display(Name = "Data Ini.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "")]
        public DateTime dataInizio { get; set; }
        [Display(Name = "Data Fin.")]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:d}", NullDisplayText = "")]
        public DateTime? dataFine { get; set; }
        [Display(Name = "Parentela")]
        public EnumParentela parentela { get; set; }
        [Display(Name = "Pensione")]
        public bool HasPensione { get; set; }
        [Display(Name = "Altri dati")]
        public decimal idAltriDati { get; set; }

        [Display(Name = "Documenti")]
        public IList<DocumentiModel> Documenti { get; set; }

    }
}