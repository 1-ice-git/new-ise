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
    
    public partial class ATTIVAZIONIPASSAPORTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVAZIONIPASSAPORTI()
        {
            this.FIGLIPASSAPORTO = new HashSet<FIGLIPASSAPORTO>();
            this.PASSAPORTORICHIEDENTE = new HashSet<PASSAPORTORICHIEDENTE>();
        }
    
        public decimal IDATTIVAZIONIPASSAPORTI { get; set; }
        public decimal IDPASSAPORTI { get; set; }
        public bool NOTIFICARICHIESTA { get; set; }
        public Nullable<System.DateTime> DATANOTIFICARICHIESTA { get; set; }
        public bool PRATICACONCLUSA { get; set; }
        public Nullable<System.DateTime> DATAPRATICACONCLUSA { get; set; }
        public System.DateTime DATAVARIAZIONE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLIPASSAPORTO> FIGLIPASSAPORTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PASSAPORTORICHIEDENTE> PASSAPORTORICHIEDENTE { get; set; }
        public virtual PASSAPORTI PASSAPORTI { get; set; }
    }
}
