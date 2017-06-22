namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.UTENTIAUTORIZZATI")]
    public partial class UTENTIAUTORIZZATI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UTENTIAUTORIZZATI()
        {
            ACCESSI = new HashSet<ACCESSI>();
            LOGATTIVITA = new HashSet<LOGATTIVITA>();
        }

        [Key]
        public decimal IDUTENTEAUTORIZZATO { get; set; }

        public decimal IDRUOLOUTENTE { get; set; }

        [Required]
        [StringLength(50)]
        public string UTENTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACCESSI> ACCESSI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOGATTIVITA> LOGATTIVITA { get; set; }

        public virtual RUOLOACCESSO RUOLOACCESSO { get; set; }
    }
}
