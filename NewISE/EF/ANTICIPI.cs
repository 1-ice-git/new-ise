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
    
    public partial class ANTICIPI
    {
        public decimal IDATTIVITAANTICIPI { get; set; }
        public decimal IDTIPOLOGIAANTICIPI { get; set; }
        public decimal PERCENTUALEANTICIPO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ATTIVITAANTICIPI ATTIVITAANTICIPI { get; set; }
        public virtual TIPOLOGIAANTICIPI TIPOLOGIAANTICIPI { get; set; }
    }
}
