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
    
    public partial class TIPOVOCE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOVOCE()
        {
            this.VOCI = new HashSet<VOCI>();
        }
    
        public decimal IDTIPOVOCE { get; set; }
        public decimal IDTIPOELABOPRAZIONE { get; set; }
        public string DESCRIZIONE { get; set; }
    
        public virtual TIPOELABORAZIONE TIPOELABORAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VOCI> VOCI { get; set; }
    }
}