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
    
    public partial class LOGATTIVITA
    {
        public decimal IDLOG { get; set; }
        public Nullable<decimal> IDTRASFERIMENTO { get; set; }
        public decimal IDATTIVITACRUD { get; set; }
        public decimal IDDIPENDENTE { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
        public string DESCATTIVITASVOLTA { get; set; }
        public string TABELLACOINVOLTA { get; set; }
        public Nullable<decimal> IDTABELLACOINVOLTA { get; set; }
    
        public virtual ATTIVITACRUD ATTIVITACRUD { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        public virtual UTENTIAUTORIZZATI UTENTIAUTORIZZATI { get; set; }
    }
}
