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
    
    public partial class ATTIVITATERIENTRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVITATERIENTRO()
        {
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDATERIENTRO { get; set; }
        public decimal IDTERIENTRO { get; set; }
        public decimal IDANTICIPOSALDOTE { get; set; }
        public bool RICHIESTATRASPORTOEFFETTI { get; set; }
        public Nullable<System.DateTime> DATARICHIESTATE { get; set; }
        public bool ATTIVAZIONETRASPORTOEFFETTI { get; set; }
        public Nullable<System.DateTime> DATAATTIVAZIONETE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ANTICIPOSALDOTE ANTICIPOSALDOTE { get; set; }
        public virtual RINUNCIA_TE_R RINUNCIA_TE_R { get; set; }
        public virtual TERIENTRO TERIENTRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
