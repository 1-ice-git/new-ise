namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TRASFERIMENTO")]
    public partial class TRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRASFERIMENTO()
        {
            LOGATTIVITA = new HashSet<LOGATTIVITA>();
            MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
            MAGGIORAZIONECONIUGE = new HashSet<MAGGIORAZIONECONIUGE>();
            MAGGIORAZIONEFIGLI = new HashSet<MAGGIORAZIONEFIGLI>();
            NOTIFICARICHIESTAMAGFAM = new HashSet<NOTIFICARICHIESTAMAGFAM>();
            PRIMASITEMAZIONE = new HashSet<PRIMASITEMAZIONE>();
            RICHIAMO = new HashSet<RICHIAMO>();
            SOSPENSIONE = new HashSet<SOSPENSIONE>();
            DOCUMENTI = new HashSet<DOCUMENTI>();
        }

        [Key]
        public decimal IDTRASFERIMENTO { get; set; }

        public decimal IDTIPOTRASFERIMENTO { get; set; }

        public decimal IDUFFICIO { get; set; }

        public decimal IDSTATOTRASFERIMENTO { get; set; }

        public decimal IDDIPENDENTE { get; set; }

        public decimal IDTIPOCOAN { get; set; }

        public DateTime DATAPARTENZA { get; set; }

        public DateTime? DATARIENTRO { get; set; }

        [StringLength(10)]
        public string COAN { get; set; }

        [StringLength(100)]
        public string PROTOCOLLOLETTERA { get; set; }

        public DateTime? DATALETTERA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public bool NOTIFICATRASFERIMENTO { get; set; }

        public virtual DIPENDENTI DIPENDENTI { get; set; }

        public virtual INDENNITA INDENNITA { get; set; }

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
        public virtual ICollection<SOSPENSIONE> SOSPENSIONE { get; set; }

        public virtual STATOTRASFERIMENTO STATOTRASFERIMENTO { get; set; }

        public virtual TIPOLOGIACOAN TIPOLOGIACOAN { get; set; }

        public virtual TIPOTRASFERIMENTO TIPOTRASFERIMENTO { get; set; }

        public virtual UFFICI UFFICI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTI> DOCUMENTI { get; set; }
    }
}
