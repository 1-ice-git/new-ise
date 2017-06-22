namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAGGIORAZIONEFIGLI")]
    public partial class MAGGIORAZIONEFIGLI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONEFIGLI()
        {
            FIGLI = new HashSet<FIGLI>();
            MAGFAM_DOC = new HashSet<MAGFAM_DOC>();
            INDENNITA = new HashSet<INDENNITA>();
        }

        [Key]
        public decimal IDMAGGIORAZIONEFIGLI { get; set; }

        public decimal IDPERCMAGFIGLI { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public decimal IDINDPRIMOSEGR { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FIGLI> FIGLI { get; set; }

        public virtual INDENNITAPRIMOSEGRETARIO INDENNITAPRIMOSEGRETARIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGFAM_DOC> MAGFAM_DOC { get; set; }

        public virtual PERCENTUALEMAGFIGLI PERCENTUALEMAGFIGLI { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
    }
}
