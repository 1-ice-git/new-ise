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
    
    public partial class ELABINDSISTEMAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELABINDSISTEMAZIONE()
        {
            this.ELABDATIFIGLI = new HashSet<ELABDATIFIGLI>();
            this.TEORICI = new HashSet<TEORICI>();
            this.ALIQUOTECONTRIBUTIVE = new HashSet<ALIQUOTECONTRIBUTIVE>();
        }
    
        public decimal IDINDSISTLORDA { get; set; }
        public decimal IDPRIMASISTEMAZIONE { get; set; }
        public decimal IDLIVELLO { get; set; }
        public decimal INDENNITABASE { get; set; }
        public decimal COEFFICENTESEDE { get; set; }
        public decimal PERCENTUALEDISAGIO { get; set; }
        public decimal COEFFICENTEINDSIST { get; set; }
        public decimal PERCENTUALERIDUZIONE { get; set; }
        public decimal PERCENTUALEMAGCONIUGE { get; set; }
        public decimal PENSIONECONIUGE { get; set; }
        public bool ANTICIPO { get; set; }
        public bool SALDO { get; set; }
        public bool UNICASOLUZIONE { get; set; }
        public decimal PERCANTSALDOUNISOL { get; set; }
        public bool CONGUAGLIO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABDATIFIGLI> ELABDATIFIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        public virtual LIVELLI LIVELLI { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
    }
}
