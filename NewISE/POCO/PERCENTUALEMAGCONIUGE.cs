namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.PERCENTUALEMAGCONIUGE")]
    public partial class PERCENTUALEMAGCONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAGCONIUGE()
        {
            MAGGIORAZIONECONIUGE = new HashSet<MAGGIORAZIONECONIUGE>();
        }

        [Key]
        public decimal IDPERCMAGCONIUGE { get; set; }

        public decimal IDTIPOLOGIACONIUGE { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal PERCENTUALECONIUGE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONECONIUGE> MAGGIORAZIONECONIUGE { get; set; }

        public virtual TIPOLOGIACONIUGE TIPOLOGIACONIUGE { get; set; }
    }
}
