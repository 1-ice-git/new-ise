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
    
    public partial class ANTICIPI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ANTICIPI()
        {
            this.ELAB_CONT = new HashSet<ELAB_CONT>();
        }
    
        public decimal IDANTICIPO { get; set; }
        public decimal IDPRIMASISTEMAZIONE { get; set; }
        public decimal IMPORTOANTICIPO { get; set; }
        public decimal NOTIFICARICHIESTA { get; set; }
        public decimal PRATICACONCLUSA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public Nullable<System.DateTime> DATANOTIFICARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATAPRATICACONCLUSA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
    }
}
