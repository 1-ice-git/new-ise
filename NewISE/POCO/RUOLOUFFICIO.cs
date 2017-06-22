namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.RUOLOUFFICIO")]
    public partial class RUOLOUFFICIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUOLOUFFICIO()
        {
            RUOLODIPENDENTE = new HashSet<RUOLODIPENDENTE>();
        }

        [Key]
        public decimal IDRUOLO { get; set; }

        [Required]
        [StringLength(30)]
        public string DESCRUOLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }
    }
}
