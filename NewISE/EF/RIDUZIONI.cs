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
    
    public partial class RIDUZIONI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RIDUZIONI()
        {
            this.INDENNITABASE = new HashSet<INDENNITABASE>();
            this.INDENNITASISTEMAZIONE = new HashSet<INDENNITASISTEMAZIONE>();
        }
    
        public decimal IDRIDUZIONI { get; set; }
        public decimal IDREGOLA { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual REGOLECALCOLO REGOLECALCOLO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITABASE> INDENNITABASE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITASISTEMAZIONE> INDENNITASISTEMAZIONE { get; set; }
    }
}
