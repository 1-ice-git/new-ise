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
    
    public partial class PAGATOCONDIVISOMAB
    {
        public decimal IDPAGATOCONDIVISO { get; set; }
        public decimal IDMAB { get; set; }
        public decimal IDATTIVAZIONEMAB { get; set; }
        public System.DateTime DATAINIZIOVALIDITA { get; set; }
        public System.DateTime DATAFINEVALIDITA { get; set; }
        public bool CONDIVISO { get; set; }
        public bool PAGATO { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual ATTIVAZIONEMAB ATTIVAZIONEMAB { get; set; }
        public virtual MAGGIORAZIONEABITAZIONE MAGGIORAZIONEABITAZIONE { get; set; }
    }
}
