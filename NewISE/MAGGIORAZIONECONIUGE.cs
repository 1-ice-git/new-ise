//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE
{
    using System;
    using System.Collections.Generic;
    
    public partial class MAGGIORAZIONECONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONECONIUGE()
        {
            this.CONIUGE = new HashSet<CONIUGE>();
            this.INDENNITA = new HashSet<INDENNITA>();
        }
    
        public decimal IDMAGGIORAZIONECONIUGE { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDPERCMAGCONIUGE { get; set; }
        public Nullable<decimal> IDPENSIONECONIUGE { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
        public virtual PENSIONECONIUGE PENSIONECONIUGE { get; set; }
        public virtual PERCENTUALEMAGCONIUGE PERCENTUALEMAGCONIUGE { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
