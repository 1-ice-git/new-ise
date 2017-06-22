namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ATTIVITACRUD")]
    public partial class ATTIVITACRUD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ATTIVITACRUD()
        {
            LOGATTIVITA = new HashSet<LOGATTIVITA>();
        }

        [Key]
        public decimal IDATTIVITACRUD { get; set; }

        [Required]
        [StringLength(30)]
        public string DESCRIZIONEATTIVITA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOGATTIVITA> LOGATTIVITA { get; set; }
    }
}
