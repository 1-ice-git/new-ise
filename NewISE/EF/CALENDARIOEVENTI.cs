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
    
    public partial class CALENDARIOEVENTI
    {
        public decimal IDCALENDARIOEVENTI { get; set; }
        public decimal IDFUNZIONIEVENTI { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public System.DateTime DATAINIZIOEVENTO { get; set; }
        public System.DateTime DATASCADENZA { get; set; }
        public bool COMPLETATO { get; set; }
        public Nullable<System.DateTime> DATACOMPLETATO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual FUNZIONIEVENTI FUNZIONIEVENTI { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
