using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class IndennitaModel
    {
        [Key]
        public decimal idTrasfIndennita { get; set; }

        [Required(ErrorMessage = "La data di inizio è richiesta.")]
        [Display(Name = "Data inizio")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime dataInizio { get; set; }

        [Display(Name = "Data fine")]
        [DataType(DataType.DateTime, ErrorMessage = "la data non è valida.")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? dataFine { get; set; }

        [Required(ErrorMessage = "La data di aggiornamento è richiesta.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data Lettera")]
        public DateTime dataAggiornamento { get; set; }

        public TrasferimentoModel Trasferimento { get; set; }

        public IList<RuoloDipendenteModel> RuoloDipendente { get; set; }

        public IList<LivelloDipendenteModel> LivelloDipendente { get; set; }

        public IList<IndennitaBaseModel> IndennitaBase { get; set; }

        public IList<TFRModel> TFR { get; set; }

        public IList<PercentualeDisagioModel> PercentualeDisagio { get; set; }

        public IList<CoefficientiSedeModel> CoefficenteSede { get; set; }

        public IList<MaggiorazioniFamiliariModel> MaggiorazioneConiuge { get; set; }

        public IList<MaggiorazioniFigliModel> MaggiorazioneFigli { get; set; }
    }
}