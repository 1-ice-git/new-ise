namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOLOGIAFIGLIO")]
    public partial class TIPOLOGIAFIGLIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOLOGIAFIGLIO()
        {
            PERCENTUALEMAGFIGLI = new HashSet<PERCENTUALEMAGFIGLI>();
        }

        [Key]
        public decimal IDTIPOLOGIAFIGLIO { get; set; }

        [Column("TIPOLOGIAFIGLIO")]
        [Required]
        [StringLength(30)]
        public string TIPOLOGIAFIGLIO1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAGFIGLI> PERCENTUALEMAGFIGLI { get; set; }
    }
}
