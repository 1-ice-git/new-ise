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
    
    public partial class CANONEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CANONEMAB()
        {
            this.CANONEMAB1 = new HashSet<CANONEMAB>();
            this.TFR = new HashSet<TFR>();
        }
    
        public decimal IDCANONE { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public decimal IDVALUTA { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal IMPORTOCANONE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool NASCONDI { get; set; }
        public Nullable<decimal> FK_IDCANONE { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CANONEMAB> CANONEMAB1 { get; set; }
        public virtual CANONEMAB CANONEMAB2 { get; set; }
        public virtual MAB MAB { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        public virtual VALUTE VALUTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TFR> TFR { get; set; }
    }
}
