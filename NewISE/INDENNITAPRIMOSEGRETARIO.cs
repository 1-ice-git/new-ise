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
    
    public partial class INDENNITAPRIMOSEGRETARIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITAPRIMOSEGRETARIO()
        {
            this.MF_IPS = new HashSet<MF_IPS>();
        }
    
        public decimal IDINDPRIMOSEGR { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public Nullable<System.DateTime> DATAFINEVALIDITA { get; set; }
        public decimal INDENNITA { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MF_IPS> MF_IPS { get; set; }
    }
}