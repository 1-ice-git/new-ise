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
    
    public partial class PERCENTUALEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAB()
        {
            this.ANTICIPOANNUALEMAB = new HashSet<ANTICIPOANNUALEMAB>();
        }
    
        public decimal IDPERCMAB { get; set; }
        public decimal IDUFFICIO { get; set; }
        public decimal IDLIVELLO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALE { get; set; }
        public decimal PERCENTUALERESPONSABILE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual LIVELLI LIVELLI { get; set; }
        public virtual UFFICI UFFICI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPOANNUALEMAB> ANTICIPOANNUALEMAB { get; set; }
    }
}
