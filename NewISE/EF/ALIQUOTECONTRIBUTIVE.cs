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
    
    public partial class ALIQUOTECONTRIBUTIVE
    {
        public decimal IDALIQCONTR { get; set; }
        public decimal IDTIPOCONTRIBUTO { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public decimal ALIQUOTA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual MAB_ALIQCONTR MAB_ALIQCONTR { get; set; }
        public virtual TIPOALIQUOTECONTRIBUTIVE TIPOALIQUOTECONTRIBUTIVE { get; set; }
    }
}