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
    
    public partial class TEORICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TEORICI()
        {
            this.ELABINDENNITA = new HashSet<ELABINDENNITA>();
            this.ELABMAB = new HashSet<ELABMAB>();
        }
    
        public decimal IDTEORICI { get; set; }
        public decimal IDTIPOMOVIMENTO { get; set; }
        public decimal IDVOCI { get; set; }
        public Nullable<decimal> IDELABTRASPEFFETTI { get; set; }
        public decimal IDMESEANNOELAB { get; set; }
        public decimal MESERIFERIMENTO { get; set; }
        public decimal ANNORIFERIMENTO { get; set; }
        public decimal ALIQUOTAFISCALE { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool INSERIMENTOMANUALE { get; set; }
        public bool ANNULLATO { get; set; }
        public decimal IMPORTO { get; set; }
        public bool ELABORATO { get; set; }
        public Nullable<decimal> IDELABINDRICHIAMO { get; set; }
        public bool DIRETTO { get; set; }
        public Nullable<decimal> IDINDSISTLORDA { get; set; }
        public Nullable<decimal> IDAUTOVOCIMANUALI { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
    
        public virtual AUTOMATISMOVOCIMANUALI AUTOMATISMOVOCIMANUALI { get; set; }
        public virtual ELABINDRICHIAMO ELABINDRICHIAMO { get; set; }
        public virtual ELABINDSISTEMAZIONE ELABINDSISTEMAZIONE { get; set; }
        public virtual ELABTRASPEFFETTI ELABTRASPEFFETTI { get; set; }
        public virtual FLUSSICEDOLINO FLUSSICEDOLINO { get; set; }
        public virtual MESEANNOELABORAZIONE MESEANNOELABORAZIONE { get; set; }
        public virtual OA OA { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        public virtual TIPOMOVIMENTO TIPOMOVIMENTO { get; set; }
        public virtual VOCI VOCI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABINDENNITA> ELABINDENNITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELABMAB> ELABMAB { get; set; }
    }
}
