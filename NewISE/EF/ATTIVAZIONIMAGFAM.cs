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
    
    public partial class ATTIVAZIONIMAGFAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVAZIONIMAGFAM()
        {
            this.CONIUGE = new HashSet<CONIUGE>();
            this.FIGLI = new HashSet<FIGLI>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDATTIVAZIONEMAGFAM { get; set; }
        public decimal IDMAGGIORAZIONIFAMILIARI { get; set; }
        public bool RICHIESTAATTIVAZIONE { get; set; }
        public Nullable<System.DateTime> DATARICHIESTAATTIVAZIONE { get; set; }
        public bool ATTIVAZIONEMAGFAM { get; set; }
        public Nullable<System.DateTime> DATAATTIVAZIONEMAGFAM { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public System.DateTime DATAVARIAZIONE { get; set; }
    
        public virtual MAGGIORAZIONIFAMILIARI MAGGIORAZIONIFAMILIARI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLI> FIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
