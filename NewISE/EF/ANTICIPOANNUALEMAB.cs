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
    
    public partial class ANTICIPOANNUALEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ANTICIPOANNUALEMAB()
        {
            this.ANTICIPOANNUALEMAB1 = new HashSet<ANTICIPOANNUALEMAB>();
        }
    
        public decimal IDANTICIPOANNUALEMAB { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public bool ANTICIPOANNUALE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public Nullable<decimal> FK_IDANTICIPOANNUALEMAB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPOANNUALEMAB> ANTICIPOANNUALEMAB1 { get; set; }
        public virtual ANTICIPOANNUALEMAB ANTICIPOANNUALEMAB2 { get; set; }
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        public virtual MAB MAB { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
    }
}
