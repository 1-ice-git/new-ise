//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class PAGATOCONDIVISOMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PAGATOCONDIVISOMAB()
        {
            this.PAGATOCONDIVISOMAB1 = new HashSet<PAGATOCONDIVISOMAB>();
            this.PERCENTUALECONDIVISIONE = new HashSet<PERCENTUALECONDIVISIONE>();
        }
    
        public decimal IDPAGATOCONDIVISO { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDSTATORECORD { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public bool CONDIVISO { get; set; }
        public bool PAGATO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool NASCONDI { get; set; }
        public Nullable<decimal> FK_IDPAGATOCONDIVISO { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        public virtual MAB MAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB1 { get; set; }
        public virtual PAGATOCONDIVISOMAB PAGATOCONDIVISOMAB2 { get; set; }
        public virtual STATORECORD STATORECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALECONDIVISIONE> PERCENTUALECONDIVISIONE { get; set; }
    }
}
