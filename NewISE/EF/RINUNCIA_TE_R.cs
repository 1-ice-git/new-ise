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
    
    public partial class RINUNCIA_TE_R
    {
        public decimal IDATERIENTRO { get; set; }
        public bool RINUNCIATE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
    
        public virtual ATTIVITATERIENTRO ATTIVITATERIENTRO { get; set; }
    }
}
