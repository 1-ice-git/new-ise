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
    
    public partial class VALUTAUFFICIO
    {
        public decimal IDVALUTAUFFICIO { get; set; }
        public decimal IDVALUTA { get; set; }
        public decimal IDUFFICIO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual UFFICI UFFICI { get; set; }
        public virtual VALUTE VALUTE { get; set; }
    }
}
