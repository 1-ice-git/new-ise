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
    
    public partial class FASCKM_CFKM
    {
        public decimal IDFASCIAKM { get; set; }
        public decimal IDCFKM { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual COEFFICENTEFKM COEFFICENTEFKM { get; set; }
        public virtual FASCIACHILOMETRICA FASCIACHILOMETRICA { get; set; }
    }
}
