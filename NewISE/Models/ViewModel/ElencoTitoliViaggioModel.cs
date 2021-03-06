﻿using NewISE.Models.DBModel.dtObj;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NewISE.Models.Enumeratori;

namespace NewISE.Models.DBModel
{
    public class ElencoTitoliViaggioModel
    {
        public decimal idAttivazioneTitoloViaggio { get; set; }
        public decimal idFamiliare { get; set; }///ID del figlio, del coniuge
        public decimal? idTitoloViaggio { get; set; }
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

        public decimal idParentela { get; set; }

        public decimal idAltriDati { get; set; }

        public decimal idDocIdentita { get; set; }

        public decimal idDocCartaImbarco { get; set; }

        public decimal idDoctitoloViaggio { get; set; }

        public bool esisteDocCartaImbarco { get; set; }

        public bool esisteDoctitoloViaggio { get; set; }

        [Display(Name = "Richiedi")]
        [DefaultValue(false)]
        public bool RichiediTitoloViaggio { get; set; }

        public decimal idFigliTitoloViaggio { get; set; }

        public decimal idConiugeTitoloViaggio { get; set; }

    }
}