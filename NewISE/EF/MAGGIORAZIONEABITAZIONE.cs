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
    
    public partial class MAGGIORAZIONEABITAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONEABITAZIONE()
        {
            this.CANOMEMAB = new HashSet<CANOMEMAB>();
            this.MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
            this.PERCENTUALEMAB = new HashSet<PERCENTUALEMAB>();
            this.PAGATOCONDIVISOMAB = new HashSet<PAGATOCONDIVISOMAB>();
        }
    
        public decimal IDMAB { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public System.DateTime DATAINIZIOMAB { get; set; }
        public System.DateTime DATAFINEMAB { get; set; }
        public bool ANTICIPOANNUALE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CANOMEMAB> CANOMEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PAGATOCONDIVISOMAB> PAGATOCONDIVISOMAB { get; set; }
    }
}
