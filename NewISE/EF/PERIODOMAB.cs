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
    
    public partial class PERIODOMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERIODOMAB()
        {
            this.PERIODOMAB1 = new HashSet<PERIODOMAB>();
        }
    
        public decimal IDPERIODOMAB { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public System.DateTime DATAINIZIOMAB { get; set; }
        public System.DateTime DATAFINEMAB { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public Nullable<decimal> FK_IDPERIODOMAB { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        public virtual MAB MAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIODOMAB> PERIODOMAB1 { get; set; }
        public virtual PERIODOMAB PERIODOMAB2 { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
    }
}
