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
    
    public partial class PERCENTUALEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAB()
        {
            this.PERCMAB_MAB = new HashSet<PERCMAB_MAB>();
        }
    
        public decimal IDPERCMAB { get; set; }
        public decimal IDUFFICIO { get; set; }
        public decimal IDLIVELLO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public string DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALE { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual LIVELLI LIVELLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCMAB_MAB> PERCMAB_MAB { get; set; }
        public virtual UFFICI UFFICI { get; set; }
    }
}
