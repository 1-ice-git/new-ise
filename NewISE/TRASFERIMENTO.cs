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
    
    public partial class TRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASFERIMENTO()
        {
            this.INDENNITA = new HashSet<INDENNITA>();
            this.LOGATTIVITA = new HashSet<LOGATTIVITA>();
            this.MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
            this.MAGGIORAZIONECONIUGE = new HashSet<MAGGIORAZIONECONIUGE>();
            this.MAGGIORAZIONEFIGLI = new HashSet<MAGGIORAZIONEFIGLI>();
            this.NOTIFICARICHIESTAMAGFAM = new HashSet<NOTIFICARICHIESTAMAGFAM>();
            this.PRIMASITEMAZIONE = new HashSet<PRIMASITEMAZIONE>();
            this.RICHIAMO = new HashSet<RICHIAMO>();
            this.RUOLODIPENDENTE = new HashSet<RUOLODIPENDENTE>();
            this.SOSPENSIONE = new HashSet<SOSPENSIONE>();
            this.DOCUMENTI = new HashSet<DOCUMENTI>();
        }
    
        public decimal IDTRASFERIMENTO { get; set; }
        public decimal IDTIPOTRASFERIMENTO { get; set; }
        public decimal IDUFFICIO { get; set; }
        public decimal IDSTATOTRASFERIMENTO { get; set; }
        public decimal IDDIPENDENTE { get; set; }
        public decimal IDTIPOCOAN { get; set; }
        public System.DateTime DATAPARTENZA { get; set; }
        public Nullable<System.DateTime> DATARIENTRO { get; set; }
        public string COAN { get; set; }
        public string PROTOCOLLOLETTERA { get; set; }
        public Nullable<System.DateTime> DATALETTERA { get; set; }
        public System.DateTime DATAAGGIORNAMENTO { get; set; }
        public bool ANNULLATO { get; set; }
    
        public virtual DIPENDENTI DIPENDENTI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOGATTIVITA> LOGATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONECONIUGE> MAGGIORAZIONECONIUGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEFIGLI> MAGGIORAZIONEFIGLI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICARICHIESTAMAGFAM> NOTIFICARICHIESTAMAGFAM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRIMASITEMAZIONE> PRIMASITEMAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RICHIAMO> RICHIAMO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }
        public virtual STATOTRASFERIMENTO STATOTRASFERIMENTO { get; set; }
        public virtual TIPOLOGIACOAN TIPOLOGIACOAN { get; set; }
        public virtual TIPOTRASFERIMENTO TIPOTRASFERIMENTO { get; set; }
        public virtual UFFICI UFFICI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
