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
    
    public partial class RICHIAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RICHIAMO()
        {
            this.ELAB_CONT = new HashSet<ELAB_CONT>();
            this.INDRIC_COEFINDRICH = new HashSet<INDRIC_COEFINDRICH>();
            this.TRASPORTOEFFETTIRIENTRO = new HashSet<TRASPORTOEFFETTIRIENTRO>();
        }
    
        public decimal IDINDENNITA { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }
        public virtual INDENNITA INDENNITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDRIC_COEFINDRICH> INDRIC_COEFINDRICH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTIRIENTRO> TRASPORTOEFFETTIRIENTRO { get; set; }
    }
}