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
    
    public partial class TRASPEFFETTIRIEN_COEFFIFKM
    {
        public decimal IDTRASPORTOEFFETTISIST { get; set; }
        public decimal IDCFKM { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual COEFFICIENTEFKM COEFFICIENTEFKM { get; set; }
        public virtual TRASPORTOEFFETTIRIENTRO TRASPORTOEFFETTIRIENTRO { get; set; }
    }
}
