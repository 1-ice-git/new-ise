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
    
    public partial class CONIUGETITOLIVIAGGIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONIUGETITOLIVIAGGIO()
        {
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDCONIUGETITOLIVIAGGIO { get; set; }
        public decimal IDCONIUGE { get; set; }
        public decimal IDTITOLOVIAGGIO { get; set; }
        public decimal IDATTIVAZIONETITOLIVIAGGIO { get; set; }
        public bool RICHIEDITITOLOVIAGGIO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ATTIVAZIONETITOLIVIAGGIO ATTIVAZIONETITOLIVIAGGIO { get; set; }
        public virtual CONIUGE CONIUGE { get; set; }
        public virtual TITOLIVIAGGIO TITOLIVIAGGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
