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
    
    public partial class MAB_DOC
    {
        public decimal IDMAB { get; set; }
        public decimal IDDOCUMENTO { get; set; }
        public decimal IDTIPODOCUMENTO { get; set; }
        public decimal IDINDENNITA { get; set; }
    
        public virtual DOCUMENTI DOCUMENTI { get; set; }
        public virtual MAGGIORAZIONEABITAZIONE MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual TIPODOCUMENTI TIPODOCUMENTI { get; set; }
    }
}
