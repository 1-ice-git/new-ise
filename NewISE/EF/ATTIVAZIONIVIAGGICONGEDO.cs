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
    
    public partial class ATTIVAZIONIVIAGGICONGEDO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVAZIONIVIAGGICONGEDO()
        {
            this.ATTIVAZIONIVIAGGICONGEDO1 = new HashSet<ATTIVAZIONIVIAGGICONGEDO>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDATTIVAZIONEVC { get; set; }
        public decimal IDVIAGGICONGEDO { get; set; }
        public bool NOTIFICARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATANOTIFICARICHIESTA { get; set; }
        public bool ATTIVARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATAATTIVARICHIESTA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public Nullable<decimal> FK_IDATTIVAZIONEVC { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONIVIAGGICONGEDO> ATTIVAZIONIVIAGGICONGEDO1 { get; set; }
        public virtual ATTIVAZIONIVIAGGICONGEDO ATTIVAZIONIVIAGGICONGEDO2 { get; set; }
        public virtual VIAGGICONGEDO VIAGGICONGEDO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
