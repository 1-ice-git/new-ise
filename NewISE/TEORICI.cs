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
    
    public partial class TEORICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEORICI()
        {
            this.CONTABILITA = new HashSet<CONTABILITA>();
            this.STIPENDI = new HashSet<STIPENDI>();
        }
    
        public decimal IDTEORICI { get; set; }
        public decimal IDELABCONT { get; set; }
        public decimal IDVOCI { get; set; }
        public decimal IDTIPOMOVIMENTO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public decimal IMPORTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTABILITA> CONTABILITA { get; set; }
        public virtual ELAB_CONT ELAB_CONT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STIPENDI> STIPENDI { get; set; }
        public virtual TIPOMOVIMENTO TIPOMOVIMENTO { get; set; }
        public virtual VOCI VOCI { get; set; }
    }
}