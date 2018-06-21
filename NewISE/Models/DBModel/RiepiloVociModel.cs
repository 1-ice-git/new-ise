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

        public TipoLiquidazioneModel TipoLiquidazione { get; set; }
        public TipoVoceModel TipoVoce { get; set; }

    }
}