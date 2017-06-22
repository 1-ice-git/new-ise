namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAGGIORAZIONECONIUGE")]
    public partial class MAGGIORAZIONECONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONECONIUGE()
        {
            CONIUGE = new HashSet<CONIUGE>();
            MAGFAM_DOC = new HashSet<MAGFAM_DOC>();
            INDENNITA = new HashSet<INDENNITA>();
        }

        [Key]
        public decimal IDMAGGIORAZIONECONIUGE { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public decimal IDPERCMAGCONIUGE { get; set; }

        public decimal? IDPENSIONECONIUGE { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONIUGE> CONIUGE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGFAM_DOC> MAGFAM_DOC { get; set; }

        public virtual PENSIONECONIUGE PENSIONECONIUGE { get; set; }

        public virtual PERCENTUALEMAGCONIUGE PERCENTUALEMAGCONIUGE { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
    }
}
