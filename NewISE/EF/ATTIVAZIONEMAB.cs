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
    
    public partial class ATTIVAZIONEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVAZIONEMAB()
        {
            this.ANTICIPOANNUALEMAB = new HashSet<ANTICIPOANNUALEMAB>();
            this.CANONEMAB = new HashSet<CANONEMAB>();
            this.MODIFICHE_MAB = new HashSet<MODIFICHE_MAB>();
            this.PAGATOCONDIVISOMAB = new HashSet<PAGATOCONDIVISOMAB>();
            this.PERIODOMAB = new HashSet<PERIODOMAB>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDATTIVAZIONEMAB { get; set; }
        public bool NOTIFICARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATANOTIFICARICHIESTA { get; set; }
        public bool ATTIVAZIONE { get; set; }
        public Nullable<System.DateTime> DATAATTIVAZIONE { get; set; }
        public System.DateTime DATAVARIAZIONE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public decimal IDMAB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPOANNUALEMAB> ANTICIPOANNUALEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CANONEMAB> CANONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MODIFICHE_MAB> MODIFICHE_MAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIODOMAB> PERIODOMAB { get; set; }
        public virtual MAB MAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
