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
    
    public partial class CONIUGEPASSAPORTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONIUGEPASSAPORTO()
        {
            this.CONIUGE = new HashSet<CONIUGE>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDCONIUGEPASSAPORTO { get; set; }
        public decimal IDPASSAPORTI { get; set; }
        public decimal IDATTIVAZIONIPASSAPORTI { get; set; }
        public bool INCLUDIPASSAPORTO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ATTIVAZIONIPASSAPORTI ATTIVAZIONIPASSAPORTI { get; set; }
        public virtual PASSAPORTI PASSAPORTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
