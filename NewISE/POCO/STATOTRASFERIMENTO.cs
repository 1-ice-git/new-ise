namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.STATOTRASFERIMENTO")]
    public partial class STATOTRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STATOTRASFERIMENTO()
        {
            TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }

        [Key]
        public decimal IDSTATOTRASFERIMENTO { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
