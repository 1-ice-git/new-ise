namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOTRASFERIMENTO")]
    public partial class TIPOTRASFERIMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOTRASFERIMENTO()
        {
            INDENNITASISTEMAZIONE = new HashSet<INDENNITASISTEMAZIONE>();
            TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }

        [Key]
        public decimal IDTIPOTRASFERIMENTO { get; set; }

        [Column("TIPOTRASFERIMENTO")]
        [Required]
        [StringLength(50)]
        public string TIPOTRASFERIMENTO1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITASISTEMAZIONE> INDENNITASISTEMAZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
