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
    
    public partial class BIGLIETTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BIGLIETTI()
        {
            this.BIGLIETTI_DOCUMENTI = new HashSet<BIGLIETTI_DOCUMENTI>();
        }
    
        public decimal IDBIGLIETTO { get; set; }
        public decimal NOTIFICARICHIESTA { get; set; }
        public decimal PERSONALE { get; set; }
        public decimal PRATICACONCLUSA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIGLIETTI_DOCUMENTI> BIGLIETTI_DOCUMENTI { get; set; }
    }
}
