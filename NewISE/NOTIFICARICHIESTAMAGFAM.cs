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
    
    public partial class NOTIFICARICHIESTAMAGFAM
    {
        public decimal IDNOTRICMAGFAM { get; set; }
        public Nullable<decimal> IDMAGFAM { get; set; }
        public decimal IDINDENNITA { get; set; }
        public decimal RINUNCIAMAGGIORAZIONI { get; set; }
        public decimal PRATICACONCLUSA { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual INDENNITA INDENNITA { get; set; }
        public virtual MAGGIORAZIONIFAMILIARI MAGGIORAZIONIFAMILIARI { get; set; }
    }
}