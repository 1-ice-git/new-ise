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
    
    public partial class ATTIVITAANTICIPI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVITAANTICIPI()
        {
            this.ANTICIPI = new HashSet<ANTICIPI>();
        }
    
        public decimal IDATTIVITAANTICIPI { get; set; }
        public decimal IDPRIMASISTEMAZIONE { get; set; }
        public bool NOTIFICARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATANOTIFICARICHIESTA { get; set; }
        public bool ATTIVARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATAATTIVARICHIESTA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPI> ANTICIPI { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
    }
}
