//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class ALIQUOTECONTRIBUTIVE
    {
        public decimal IDALIQCONTR { get; set; }
        public decimal IDTIPOCONTRIBUTO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal ALIQUOTA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual MAB_ALIQCONTR MAB_ALIQCONTR { get; set; }
        public virtual TIPOALIQUOTECONTRIBUTIVE TIPOALIQUOTECONTRIBUTIVE { get; set; }
    }
}
