//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class ELABMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELABMAB()
        {
            this.ELABDATIFIGLI = new HashSet<ELABDATIFIGLI>();
            this.ALIQUOTECONTRIBUTIVE = new HashSet<ALIQUOTECONTRIBUTIVE>();
            this.TEORICI = new HashSet<TEORICI>();
        }
    
        public decimal IDELABMAB { get; set; }
        public decimal IDTRASFINDENNITA { get; set; }
        public decimal IDLIVELLO { get; set; }
        public decimal IDVALUTA { get; set; }
        public decimal INDENNITABASE { get; set; }
        public decimal COEFFICENTESEDE { get; set; }
        public decimal PERCENTUALEDISAGIO { get; set; }
        public decimal PERCENTUALERIDUZIONE { get; set; }
        public decimal PERCENTUALEMAGCONIUGE { get; set; }
        public decimal CANONELOCAZIONE { get; set; }
        public decimal TASSOFISSORAGGUAGLIO { get; set; }
        public decimal PERCMAB { get; set; }
        public System.DateTime DAL { get; set; }
        public System.DateTime AL { get; set; }
        public decimal GIORNI { get; set; }
        public bool ANNUALE { get; set; }
        public decimal PROGRESSIVO { get; set; }
        public bool CONGUAGLIO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABDATIFIGLI> ELABDATIFIGLI { get; set; }
        public virtual INDENNITA INDENNITA { get; set; }
        public virtual LIVELLI LIVELLI { get; set; }
        public virtual VALUTE VALUTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALIQUOTECONTRIBUTIVE> ALIQUOTECONTRIBUTIVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }
    }
}
