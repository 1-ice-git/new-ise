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
    
    public partial class ELABORAZIONI
    {
        public decimal IDEALBORAZIONI { get; set; }
        public decimal IDDIPENDENTE { get; set; }
        public decimal IDMESEANNOELAB { get; set; }
        public System.DateTime DATAOPERAZIONE { get; set; }
    
        public virtual DIPENDENTI DIPENDENTI { get; set; }
        public virtual MESEANNOELABORAZIONE MESEANNOELABORAZIONE { get; set; }
    }
}