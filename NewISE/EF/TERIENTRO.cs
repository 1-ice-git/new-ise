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
    
    public partial class TERIENTRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TERIENTRO()
        {
            this.ATTIVITATERIENTRO = new HashSet<ATTIVITATERIENTRO>();
            this.PERCENTUALEANTICIPOTE = new HashSet<PERCENTUALEANTICIPOTE>();
        }
    
        public decimal IDTERIENTRO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITATERIENTRO> ATTIVITATERIENTRO { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEANTICIPOTE> PERCENTUALEANTICIPOTE { get; set; }
    }
}
