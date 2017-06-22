namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOLOGIACONIUGE")]
    public partial class TIPOLOGIACONIUGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOLOGIACONIUGE()
        {
            PERCENTUALEMAGCONIUGE = new HashSet<PERCENTUALEMAGCONIUGE>();
        }

        [Key]
        public decimal IDTIPOLOGIACONIUGE { get; set; }

        [Column("TIPOLOGIACONIUGE")]
        [Required]
        [StringLength(30)]
        public string TIPOLOGIACONIUGE1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAGCONIUGE> PERCENTUALEMAGCONIUGE { get; set; }
    }
}
