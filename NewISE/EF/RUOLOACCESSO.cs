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
    
    public partial class RUOLOACCESSO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUOLOACCESSO()
        {
            this.UTENTIAUTORIZZATI = new HashSet<UTENTIAUTORIZZATI>();
        }
    
        public decimal IDRUOLOACCESSO { get; set; }
        public string DESCRUOLO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UTENTIAUTORIZZATI> UTENTIAUTORIZZATI { get; set; }
    }
}
