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
    
    public partial class MAGGIORAZIONIANNUALI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONIANNUALI()
        {
            this.DATIMAB_MABANNUALI = new HashSet<DATIMAB_MABANNUALI>();
        }
    
        public decimal IDMAGANNUALI { get; set; }
        public Nullable<decimal> IDUFFICIO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public Nullable<System.DateTime> DATAFINEVALIDITA { get; set; }
        public decimal ANNUALITA { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DATIMAB_MABANNUALI> DATIMAB_MABANNUALI { get; set; }
        public virtual UFFICI UFFICI { get; set; }
    }
}