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
    
    public partial class ELABMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELABMAB()
        {
            this.ELABMAB1 = new HashSet<ELABMAB>();
        }
    
        public decimal IDELABMAB { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDREGOLA { get; set; }
        public decimal INDENNITABASE { get; set; }
        public decimal COEFFICENTESEDE { get; set; }
        public decimal PERCENTUALEDISAGIO { get; set; }
        public decimal COEFFICENTERIDUZIONE { get; set; }
        public decimal MAGGIORAZIONECONIUGE { get; set; }
        public decimal MAGGIORAZIONEFIGLI { get; set; }
        public decimal CANONELOCAZIONE { get; set; }
        public decimal TASSOFISSORAGGUAGLIO { get; set; }
        public decimal PERCMAB { get; set; }
        public System.DateTime DAL { get; set; }
        public System.DateTime AL { get; set; }
        public bool ANNUALE { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ELABORATO { get; set; }
        public bool ANNULLATO { get; set; }
        public Nullable<decimal> FK_IDELABMAB { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABMAB> ELABMAB1 { get; set; }
        public virtual ELABMAB ELABMAB2 { get; set; }
        public virtual MAGGIORAZIONEABITAZIONE MAGGIORAZIONEABITAZIONE { get; set; }
        public virtual REGOLECALCOLO REGOLECALCOLO { get; set; }
    }
}
