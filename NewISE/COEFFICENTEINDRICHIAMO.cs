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
    
    public partial class COEFFICENTEINDRICHIAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COEFFICENTEINDRICHIAMO()
        {
            this.INDRIC_COEFINDRICH = new HashSet<INDRIC_COEFINDRICH>();
        }
    
        public decimal IDCOEFINDRICHIAMO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public Nullable<System.DateTime> DATAFINEVALIDITA { get; set; }
        public decimal COEFFICENTERICHIAMO { get; set; }
        public decimal COEFFICENTEINDBASE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDRIC_COEFINDRICH> INDRIC_COEFINDRICH { get; set; }
    }
}