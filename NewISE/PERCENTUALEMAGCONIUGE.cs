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
    
    public partial class PERCENTUALEMAGCONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAGCONIUGE()
        {
            this.MAGGIORAZIONECONIUGE = new HashSet<MAGGIORAZIONECONIUGE>();
        }
    
        public decimal IDPERCMAGCONIUGE { get; set; }
        public decimal IDTIPOLOGIACONIUGE { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALECONIUGE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONECONIUGE> MAGGIORAZIONECONIUGE { get; set; }
        public virtual TIPOLOGIACONIUGE TIPOLOGIACONIUGE { get; set; }
    }
}
