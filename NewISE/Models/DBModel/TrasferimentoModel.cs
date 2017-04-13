﻿using NewISE.Areas.Parametri.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NewISE.Models.DBModel
{
    public class TrasferimentoModel
    {
        [Key]
        public decimal idTrasferimento { get; set; }

        [Required(ErrorMessage = "Tipo trasferimento richiesto")]
        public decimal idTipoTrasferimento { get; set; }

        [Required(ErrorMessage = "Ufficio richiesto")]
        public decimal idUfficio { get; set; }

        [Required(ErrorMessage = "Stato trasferimento richiesto")]
        public decimal idStatoTrasferimento { get; set; }

        [Required(ErrorMessage = "Ruolo richiesto")]
        public decimal idRuolo { get; set; }

        [Required(ErrorMessage = "Dipendente richiesto")]
        public decimal idDipendente { get; set; }

        [Required(ErrorMessage = "Tipo CO.AN richiesto")]
        public decimal idTipoCoan { get; set; }
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
        [Display(Name = "CO.AN.")]
        [DataType(DataType.Text)]
        [CustomValidation(typeof(dtDipendenti), "MatricolaUnivoca", ErrorMessage = "La matricola inserita è già presente, inserirne un altra.")]
        public string coan { get; set; }


        [StringLength(100, ErrorMessage = "per il protocollo lettera sono richiesti un massimo di 100 caratteri.")]
        [Display(Name = "Protocollo Lettera")]
        [DataType(DataType.Text)]
        public string protocolloLettera { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime? dataLettera { get; set; }

        [Required(ErrorMessage = "Il flag annullato è richiesto.")]
        [DefaultValue(false)]
        public bool annullato { get; set; }


        public StatoTrasferimentoModel StatoTrasferimento { get; set; }

        public RuoloUfficioModel RuoloUfficio { get; set; }

        public UfficiModel Ufficio { get; set; }

        public TipoTrasferimentoModel TipoTrasferimento { get; set; }

        public DipendentiModel Dipendente { get; set; }

        public TipologiaCoanModel TipoCoan { get; set; }

        public IList<IndennitaModel> lIndennita { get; set; }
    }
}
