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
    
    public partial class PERCMAB_MAB
    {
        public decimal IDMAB { get; set; }
        public decimal IDPERCMAB { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual MAGGIORAZIONEABITAZIONE MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual PERCENTUALEMAB PERCENTUALEMAB { get; set; }
    }
}
