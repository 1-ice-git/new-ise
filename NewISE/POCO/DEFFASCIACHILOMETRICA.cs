namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.DEFFASCIACHILOMETRICA")]
    public partial class DEFFASCIACHILOMETRICA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DEFFASCIACHILOMETRICA()
        {
            COEFFICIENTEFKM = new HashSet<COEFFICIENTEFKM>();
        }

        [Key]
        public decimal IDDEFKM { get; set; }

        [Required]
        [StringLength(50)]
        public string KM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTEFKM> COEFFICIENTEFKM { get; set; }
    }
}
