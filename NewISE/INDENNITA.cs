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
            this.INDENNITA_COEFFISEDE = new HashSet<INDENNITA_COEFFISEDE>();
            this.INDENNITA_INDBASE = new HashSet<INDENNITA_INDBASE>();
            this.INDENNITA_PERCDISAGIO = new HashSet<INDENNITA_PERCDISAGIO>();
            this.INDENNITA_TFR = new HashSet<INDENNITA_TFR>();
            this.MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
            this.NOTIFICARICHIESTAMAGFAM = new HashSet<NOTIFICARICHIESTAMAGFAM>();
            this.SOSPENSIONE = new HashSet<SOSPENSIONE>();
        }
    
        public decimal IDINDENNITA { get; set; }
        public decimal IDTRASFERIMENTO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA_COEFFISEDE> INDENNITA_COEFFISEDE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA_INDBASE> INDENNITA_INDBASE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA_PERCDISAGIO> INDENNITA_PERCDISAGIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA_TFR> INDENNITA_TFR { get; set; }
        public virtual RICHIAMO RICHIAMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICARICHIESTAMAGFAM> NOTIFICARICHIESTAMAGFAM { get; set; }
        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }
        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
