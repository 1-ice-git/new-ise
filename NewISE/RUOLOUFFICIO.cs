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
    
    public partial class RUOLOUFFICIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUOLOUFFICIO()
        {
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }
    
        public decimal IDRUOLO { get; set; }
        public string DESCRUOLO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}