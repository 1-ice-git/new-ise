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
    
    public partial class SOSPENSIONE
    {
        public decimal IDSOSPENSIONE { get; set; }
        public decimal IDTIPOSOSPENSIONE { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public System.DateTime DATAINIZIO { get; set; }
        public System.DateTime DATAFINE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        public virtual TIPOSOSPENSIONE TIPOSOSPENSIONE { get; set; }
    }
}
