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
    
    public partial class RIDUZIONI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RIDUZIONI()
        {
            this.INDBASE_RID = new HashSet<INDBASE_RID>();
            this.REGOLACALCOLO_RIDUZIONI = new HashSet<REGOLACALCOLO_RIDUZIONI>();
        }
    
        public decimal IDRIDUZIONI { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public Nullable<System.DateTime> DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDBASE_RID> INDBASE_RID { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REGOLACALCOLO_RIDUZIONI> REGOLACALCOLO_RIDUZIONI { get; set; }
    }
}