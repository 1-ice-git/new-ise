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
    
    public partial class INDENNITABASE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITABASE()
        {
            this.RIDUZIONI = new HashSet<RIDUZIONI>();
            this.INDENNITA = new HashSet<INDENNITA>();
        }
    
        public decimal IDINDENNITABASE { get; set; }
        public decimal IDLIVELLO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal VALORE { get; set; }
        public decimal VALORERESP { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual LIVELLI LIVELLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RIDUZIONI> RIDUZIONI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
    }
}
