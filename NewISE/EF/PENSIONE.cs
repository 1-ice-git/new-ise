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
    
    public partial class PENSIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PENSIONE()
        {
            this.PENSIONE1 = new HashSet<PENSIONE>();
            this.ATTIVAZIONIMAGFAM = new HashSet<ATTIVAZIONIMAGFAM>();
            this.CONIUGE = new HashSet<CONIUGE>();
        }
    
        public decimal IDPENSIONE { get; set; }
        public decimal IMPORTOPENSIONE { get; set; }
        public System.DateTime DATAINIZIO { get; set; }
        public System.DateTime DATAFINE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public Nullable<decimal> FK_IDPENSIONE { get; set; }
        public bool NASCONDI { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PENSIONE> PENSIONE1 { get; set; }
        public virtual PENSIONE PENSIONE2 { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVAZIONIMAGFAM> ATTIVAZIONIMAGFAM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
    }
}
