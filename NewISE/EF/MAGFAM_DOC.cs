//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
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
