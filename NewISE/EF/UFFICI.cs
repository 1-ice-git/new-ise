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
    
    public partial class UFFICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UFFICI()
        {
            this.COEFFICIENTESEDE = new HashSet<COEFFICIENTESEDE>();
            this.MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
            this.PERCENTUALEDISAGIO = new HashSet<PERCENTUALEDISAGIO>();
            this.PERCENTUALEMAB = new HashSet<PERCENTUALEMAB>();
            this.VALUTAUFFICIO = new HashSet<VALUTAUFFICIO>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }
    
        public decimal IDUFFICIO { get; set; }
        public string CODICEUFFICIO { get; set; }
        public string DESCRIZIONEUFFICIO { get; set; }
        public bool PAGATOVALUTAUFFICIO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VALUTAUFFICIO> VALUTAUFFICIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
