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
    
    public partial class TIPOLOGIAFIGLIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOLOGIAFIGLIO()
        {
            this.FIGLI = new HashSet<FIGLI>();
            this.PERCENTUALEMAGFIGLI = new HashSet<PERCENTUALEMAGFIGLI>();
        }
    
        public decimal IDTIPOLOGIAFIGLIO { get; set; }
        public string TIPOLOGIAFIGLIO1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLI> FIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAGFIGLI> PERCENTUALEMAGFIGLI { get; set; }
    }
}
