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
    
    public partial class CDC_DIPENDENTI
    {
        public decimal ID_CDC_DIPENDENTI { get; set; }
        public decimal IDCDC { get; set; }
        public decimal IDDIPENDENTE { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual CDC CDC { get; set; }
        public virtual DIPENDENTI DIPENDENTI { get; set; }
    }
}
