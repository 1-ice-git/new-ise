//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRIMASIST_ALIQCONTR
    {
        public decimal IDALIQCONTR { get; set; }
        public decimal IDINDESTERA { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual ALIQUOTECONTRIBUTIVE ALIQUOTECONTRIBUTIVE { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
    }
}
