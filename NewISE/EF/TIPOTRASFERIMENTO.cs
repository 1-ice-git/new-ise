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
    
    public partial class TIPOTRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOTRASFERIMENTO()
        {
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
            this.INDENNITASISTEMAZIONE = new HashSet<INDENNITASISTEMAZIONE>();
        }
    
        public decimal IDTIPOTRASFERIMENTO { get; set; }
        public string TIPOTRASFERIMENTO1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITASISTEMAZIONE> INDENNITASISTEMAZIONE { get; set; }
    }
}
