﻿using NewISE.Models.DBModel.dtObj;
using NewISE.Models.Enumeratori;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{


    public class FigliModel
    {

        [Key]
        public decimal idFigli { get; set; }
        [Required(ErrorMessage = "La maggiorazione dei figli è richiesta.")]
        [Display(Name = "Magg. figli")]
        public decimal idMaggiorazioniFamiliari { get; set; }
        [Required(ErrorMessage = "La tipologia del figlio è richiesta.")]
        [Display(Name = "Tipologia")]
        public EnumTipologiaFiglio idTipologiaFiglio { get; set; }

        [Required(ErrorMessage = "Il nome è richiesto.")]
        [Display(Name = "Nome")]
        [StringLength(30, ErrorMessage = "Per il nome sono richiesti un massimo di 30 caratteri.")]
        public string nome { get; set; }
        [Required(ErrorMessage = "Il cognome è richiesto.")]
        [Display(Name = "Cognome")]
        [StringLength(30, ErrorMessage = "Per il cognome sono richiesti un massimo di 30 caratteri.")]
        public string cognome { get; set; }
        [Required(ErrorMessage = "Il codice fiscale è richiesto.")]
        [Display(Name = "Cod. Fiscale")]
        [StringLength(16, ErrorMessage = "Per il codice fiscale sono richiesti 16 caratteri.", MinimumLength = 16)]
        [CustomValidation(typeof(dtFigli), "VerificaCodiceFiscale", ErrorMessage = "")]
        public string codiceFiscale { get; set; }

        [Required(ErrorMessage = "La data di inizio validità è richiesta.")]
        [Display(Name = "Data iniz. valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [CustomValidation(typeof(dtFigli), "VerificaDataInizio")]
        public DateTime? dataInizio { get; set; }

        [CustomValidation(typeof(dtFigli), "VerificaDataFine")]
        [Display(Name = "Data fine valid.")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataFine { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [Display(Name = "Data Agg.")]
        [DataType(DataType.DateTime)]
        public DateTime dataAggiornamento { get; set; }


        [Required(ErrorMessage = "Il campo Escludi passaporto è richiesto.")]
        [Display(Name = "Escludi P.")]
        [DefaultValue(false)]
        public bool escludiPassaporto { get; set; }
        [Required(ErrorMessage = "Il campo Escludi titolo viaggio è richiesto.")]
        [Display(Name = "Escludi T.V.")]
        [DefaultValue(false)]
        public bool escludiTitoloViaggio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataNotificaPP { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime? dataNotificaTV { get; set; }

        [Display(Name = "Nominativo")]
        public string nominativo => cognome + " " + nome;


        public decimal? FK_IdFigli { get; set; }

        public decimal idAttivazioneMagFam { get; set; }

        public IList<AltriDatiFamConiugeModel> lAtriDatiFamiliari { get; set; }
        public IList<PercentualeMagFigliModel> lPercentualeMaggiorazioneFigli { get; set; }

        public PassaportoModel passaporto { get; set; }

        public MaggiorazioniFamiliariModel MaggiorazioniFamiliari { get; set; }

        public decimal idStatoRecord { get; set; }

        public bool HasValue()
        {
            return idFigli > 0 ? true : false;
        }

    }
}