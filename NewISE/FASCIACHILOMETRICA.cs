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
    
    public partial class FASCIACHILOMETRICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FASCIACHILOMETRICA()
        {
            this.FASCKM_CFKM = new HashSet<FASCKM_CFKM>();
        }
    
        public decimal IDFASCIAKM { get; set; }
        public decimal IDUFFICIO { get; set; }
        public decimal IDDEFKM { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual DEFFASCIACHILOMETRICA DEFFASCIACHILOMETRICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FASCKM_CFKM> FASCKM_CFKM { get; set; }
        public virtual UFFICI UFFICI { get; set; }
    }
}
