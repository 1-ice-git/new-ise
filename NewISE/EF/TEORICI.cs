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
    
    public partial class TEORICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEORICI()
        {
            this.CONTABILITA = new HashSet<CONTABILITA>();
            this.STIPENDI = new HashSet<STIPENDI>();
        }
    
        public decimal IDTEORICI { get; set; }
        public decimal IDVOCI { get; set; }
        public decimal IDTIPOMOVIMENTO { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public byte MESERIFERIMENTO { get; set; }
        public short ANNORIFERIMENTO { get; set; }
        public decimal IMPORTO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTABILITA> CONTABILITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STIPENDI> STIPENDI { get; set; }
        public virtual TIPOMOVIMENTO TIPOMOVIMENTO { get; set; }
        public virtual VOCI VOCI { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
