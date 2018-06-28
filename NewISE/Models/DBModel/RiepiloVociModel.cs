using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewISE.Models.DBModel
{
    public class RiepiloVociModel
    {
        [Key]
        
        [DataType(DataType.Date)]
        [Display(Name = "Data Operazione")]
        public DateTime dataOperazione { get; set; }

        [Display(Name = "Importo")]
        public decimal importo { get; set; }

        [Display(Name = "Voce")]
        public string descrizione { get; set; }

        [Display(Name = "Ind. Sist. Lorda ")]
        public string indPrimaSist { get; set; }

        [Display(Name = "Ind. Sist. Lorda ")]
        public string indSistLorda { get; set; }
        
        public TipoLiquidazioneModel TipoLiquidazione { get; set; }
        public TipoVoceModel TipoVoce { get; set; }

    }
}