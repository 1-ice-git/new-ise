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
    
    public partial class SOSPENSIONE
    {
        public decimal IDSOSPENSIONE { get; set; }
        public decimal IDTIPOSOSPENSIONE { get; set; }
        public decimal IDINDENNITA { get; set; }
        public System.DateTime DATAINIZIO { get; set; }
        public System.DateTime DATAFINE { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual INDENNITA INDENNITA { get; set; }
        public virtual TIPOSOSPENSIONE TIPOSOSPENSIONE { get; set; }
    }
}
