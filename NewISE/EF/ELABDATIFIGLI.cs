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
    
    public partial class ELABDATIFIGLI
    {
        public decimal IDELABDATIFLIGLI { get; set; }
        public Nullable<decimal> IDINDSISTLORDA { get; set; }
        public Nullable<decimal> IDELABIND { get; set; }
        public Nullable<decimal> IDELABMAB { get; set; }
        public Nullable<decimal> IDELABINDRICHIAMO { get; set; }
        public decimal INDENNITAPRIMOSEGRETARIO { get; set; }
        public decimal PERCENTUALEMAGGIORAZIONEFIGLI { get; set; }
    
        public virtual ELABINDENNITA ELABINDENNITA { get; set; }
        public virtual ELABINDRICHIAMO ELABINDRICHIAMO { get; set; }
        public virtual ELABINDSISTEMAZIONE ELABINDSISTEMAZIONE { get; set; }
        public virtual ELABMAB ELABMAB { get; set; }
    }
}
