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
    
    public partial class AUTOMATISMOVOCIMANUALI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AUTOMATISMOVOCIMANUALI()
        {
            this.TEORICI = new HashSet<TEORICI>();
        }
    
        public decimal IDAUTOVOCIMANUALI { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDVOCI { get; set; }
        public decimal ANNOMESEINIZIO { get; set; }
        public decimal ANNOMESEFINE { get; set; }
        public System.DateTime DATAINSERIMENTO { get; set; }
        public decimal IMPORTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        public virtual VOCI VOCI { get; set; }
    }
}
