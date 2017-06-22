namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.INDENNITABASE")]
    public partial class INDENNITABASE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITABASE()
        {
            INDENNITA = new HashSet<INDENNITA>();
        }

        [Key]
        public decimal IDINDENNITABASE { get; set; }

        public decimal IDLIVELLO { get; set; }

        public decimal IDRIDUZIONI { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal VALORE { get; set; }

        public decimal VALORERESP { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual LIVELLI LIVELLI { get; set; }

        public virtual RIDUZIONI RIDUZIONI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITA> INDENNITA { get; set; }
    }
}
