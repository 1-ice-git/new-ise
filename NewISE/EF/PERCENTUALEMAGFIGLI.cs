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
    
    public partial class PERCENTUALEMAGFIGLI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAGFIGLI()
        {
            this.MAGGIORAZIONEFIGLI = new HashSet<MAGGIORAZIONEFIGLI>();
        }
    
        public decimal IDPERCMAGFIGLI { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal PERCENTUALEFIGLI { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
        public decimal IDTIPOLOGIAFIGLIO { get; set; }
    
        public virtual TIPOLOGIAFIGLIO TIPOLOGIAFIGLIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEFIGLI> MAGGIORAZIONEFIGLI { get; set; }
    }
}
