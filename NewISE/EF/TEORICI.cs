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
        public decimal IDTEORICI { get; set; }
        public Nullable<decimal> IDINDSISTLORDA { get; set; }
        public decimal IDTIPOMOVIMENTO { get; set; }
        public decimal IDVOCI { get; set; }
        public Nullable<decimal> IDELABIND { get; set; }
        public Nullable<decimal> IDELABMAB { get; set; }
        public Nullable<decimal> IDELABTRASPEFFETTI { get; set; }
        public decimal IDMESEANNOELAB { get; set; }
        public decimal MESERIFERIMENTO { get; set; }
        public decimal ANNORIFERIMENTO { get; set; }
        public decimal ALIQUOTAFISCALE { get; set; }
        public decimal GIORNI { get; set; }
        public decimal IMPORTO { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual CONT_OA CONT_OA { get; set; }
        public virtual ELABINDENNITA ELABINDENNITA { get; set; }
        public virtual ELABINDSISTEMAZIONE ELABINDSISTEMAZIONE { get; set; }
        public virtual ELABMAB ELABMAB { get; set; }
        public virtual ELABTRASPEFFETTI ELABTRASPEFFETTI { get; set; }
        public virtual FLUSSICEDOLINO FLUSSICEDOLINO { get; set; }
        public virtual MESEANNOELABORAZIONE MESEANNOELABORAZIONE { get; set; }
        public virtual TIPOMOVIMENTO TIPOMOVIMENTO { get; set; }
        public virtual VOCI VOCI { get; set; }
    }
}
