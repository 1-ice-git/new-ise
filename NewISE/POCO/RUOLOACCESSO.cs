namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.RUOLOACCESSO")]
    public partial class RUOLOACCESSO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUOLOACCESSO()
        {
            UTENTIAUTORIZZATI = new HashSet<UTENTIAUTORIZZATI>();
        }

        [Key]
        public decimal IDRUOLOACCESSO { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCRUOLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UTENTIAUTORIZZATI> UTENTIAUTORIZZATI { get; set; }
    }
}
