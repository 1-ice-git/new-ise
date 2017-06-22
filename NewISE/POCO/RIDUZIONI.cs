namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.RIDUZIONI")]
    public partial class RIDUZIONI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RIDUZIONI()
        {
            INDENNITABASE = new HashSet<INDENNITABASE>();
        }

        [Key]
        public decimal IDRIDUZIONI { get; set; }

        public decimal IDREGOLA { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal PERCENTUALE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITABASE> INDENNITABASE { get; set; }

        public virtual REGOLECALCOLO REGOLECALCOLO { get; set; }
    }
}
