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
    
    public partial class MODIFICHE_MAB
    {
        public decimal IDMODMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public decimal IDMSGVAR { get; set; }
        public string VALORE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        public virtual TIPOLOGIAMODIFICHE TIPOLOGIAMODIFICHE { get; set; }
    }
}