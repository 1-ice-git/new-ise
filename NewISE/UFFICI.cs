//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NewISE
{
    using System;
    using System.Collections.Generic;
    
    public partial class UFFICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UFFICI()
        {
            this.COEFFICENTISEDE = new HashSet<COEFFICENTISEDE>();
            this.FASCIACHILOMETRICA = new HashSet<FASCIACHILOMETRICA>();
            this.MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
            this.PERCENTUALEDISAGIO = new HashSet<PERCENTUALEDISAGIO>();
            this.PERCENTUALEMAB = new HashSet<PERCENTUALEMAB>();
            this.TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
            this.VALUTAUFFICIO = new HashSet<VALUTAUFFICIO>();
        }
    
        public decimal IDUFFICIO { get; set; }
        public string CODICEUFFICIO { get; set; }
        public string DESCRIZIONEUFFICIO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICENTISEDE> COEFFICENTISEDE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FASCIACHILOMETRICA> FASCIACHILOMETRICA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VALUTAUFFICIO> VALUTAUFFICIO { get; set; }
    }
}
