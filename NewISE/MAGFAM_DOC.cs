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
    
    public partial class MAGFAM_DOC
    {
        public decimal IDDOCUMENTO { get; set; }
        public Nullable<decimal> IDMAGGIORAZIONECONIUGE { get; set; }
        public Nullable<decimal> IDMAGGIORAZIONEFIGLI { get; set; }
    
        public virtual DOCUMENTI DOCUMENTI { get; set; }
        public virtual MAGGIORAZIONECONIUGE MAGGIORAZIONECONIUGE { get; set; }
        public virtual MAGGIORAZIONEFIGLI MAGGIORAZIONEFIGLI { get; set; }
    }
}