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
    
    public partial class TRASPORTOEFFETTISIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASPORTOEFFETTISIST()
        {
            this.TRASPEFFETTISIST_DOC = new HashSet<TRASPEFFETTISIST_DOC>();
        }
    
        public decimal IDTRASPORTOEFFETTISIST { get; set; }
        public decimal IDCFKM { get; set; }
        public Nullable<decimal> IDPRIMASISTEMAZIONE { get; set; }
    
        public virtual COEFFICENTEFKM COEFFICENTEFKM { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPEFFETTISIST_DOC> TRASPEFFETTISIST_DOC { get; set; }
        public virtual TRASPORTOEFFETTIRIENTRO TRASPORTOEFFETTIRIENTRO { get; set; }
    }
}
