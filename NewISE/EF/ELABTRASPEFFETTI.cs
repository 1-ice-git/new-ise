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
    
    public partial class ELABTRASPEFFETTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELABTRASPEFFETTI()
        {
            this.TEORICI = new HashSet<TEORICI>();
        }
    
        public decimal IDELABTRASPEFFETTI { get; set; }
        public Nullable<decimal> IDTEPARTENZA { get; set; }
        public Nullable<decimal> IDTERIENTRO { get; set; }
        public Nullable<decimal> IDINDSISTLORDA { get; set; }
        public decimal COEFFICENTEFK { get; set; }
        public bool ANTICIPO { get; set; }
        public bool SALDO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ELABORATO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ELABINDSISTEMAZIONE ELABINDSISTEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        public virtual TEPARTENZA TEPARTENZA { get; set; }
        public virtual TERIENTRO TERIENTRO { get; set; }
    }
}
