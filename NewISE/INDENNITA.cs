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
    
    public partial class INDENNITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITA()
        {
            this.ELAB_CONT = new HashSet<ELAB_CONT>();
            this.RICHIAMO = new HashSet<RICHIAMO>();
            this.NOTIFICARICHIESTAMAGFAM = new HashSet<NOTIFICARICHIESTAMAGFAM>();
            this.PRIMASITEMAZIONE = new HashSet<PRIMASITEMAZIONE>();
            this.SOSPENSIONE = new HashSet<SOSPENSIONE>();
            this.VARIAZIONIRATEMAB = new HashSet<VARIAZIONIRATEMAB>();
        }
    
        public decimal IDINDENNITA { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDINDENNITABASE { get; set; }
        public decimal IDTFR { get; set; }
        public decimal IDPERCENTUALEDISAGIO { get; set; }
        public decimal IDCOEFFICENTESEDE { get; set; }
        public System.DateTime DATAINIZIO { get; set; }
        public System.DateTime DATAFINE { get; set; }
        public bool ANNULLATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RICHIAMO> RICHIAMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICARICHIESTAMAGFAM> NOTIFICARICHIESTAMAGFAM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRIMASITEMAZIONE> PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VARIAZIONIRATEMAB> VARIAZIONIRATEMAB { get; set; }
        public virtual INDENNITABASE INDENNITABASE { get; set; }
        public virtual PERCENTUALEDISAGIO PERCENTUALEDISAGIO { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
        public virtual TFR TFR { get; set; }
    }
}
