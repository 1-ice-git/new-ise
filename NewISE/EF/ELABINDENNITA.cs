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
    
    public partial class ELABINDENNITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELABINDENNITA()
        {
            this.ELABDATIFIGLI = new HashSet<ELABDATIFIGLI>();
            this.TEORICI = new HashSet<TEORICI>();
            this.ALIQUOTECONTRIBUTIVE = new HashSet<ALIQUOTECONTRIBUTIVE>();
        }
    
        public decimal IDELABIND { get; set; }
        public decimal IDTRASFINDENNITA { get; set; }
        public decimal IDLIVELLO { get; set; }
        public decimal INDENNITABASE { get; set; }
        public decimal COEFFICENTESEDE { get; set; }
        public decimal PERCENTUALEDISAGIO { get; set; }
        public decimal PERCENTUALEMAGCONIUGE { get; set; }
        public decimal PENSIONECONIUGE { get; set; }
        public decimal ALIQUOTAFISCALE { get; set; }
        public System.DateTime DAL { get; set; }
        public System.DateTime AL { get; set; }
        public decimal GIORNI { get; set; }
        public decimal GIORNISOSPENSIONE { get; set; }
        public decimal PROGRESSIVO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABDATIFIGLI> ELABDATIFIGLI { get; set; }
        public virtual INDENNITA INDENNITA { get; set; }
        public virtual LIVELLI LIVELLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
    }
}
