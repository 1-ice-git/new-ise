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
    
    public partial class INDENNITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITA()
        {
            this.COEFFICIENTESEDE = new HashSet<COEFFICIENTESEDE>();
            this.INDENNITABASE = new HashSet<INDENNITABASE>();
            this.LIVELLIDIPENDENTI = new HashSet<LIVELLIDIPENDENTI>();
            this.PERCENTUALEDISAGIO = new HashSet<PERCENTUALEDISAGIO>();
            this.RUOLODIPENDENTE = new HashSet<RUOLODIPENDENTE>();
            this.TFR = new HashSet<TFR>();
        }
    
        public decimal IDTRASFINDENNITA { get; set; }
        public System.DateTime DATAINIZIO { get; set; }
        public System.DateTime DATAFINE { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
    
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITABASE> INDENNITABASE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TFR> TFR { get; set; }
    }
}