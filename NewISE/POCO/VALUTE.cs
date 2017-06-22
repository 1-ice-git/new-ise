namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.VALUTE")]
    public partial class VALUTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VALUTE()
        {
            TFR = new HashSet<TFR>();
            UFFICI = new HashSet<UFFICI>();
        }

        [Key]
        public decimal IDVALUTA { get; set; }

        [Required]
        [StringLength(30)]
        public string DESCRIZIONEVALUTA { get; set; }

        public bool VALUTAUFFICIALE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TFR> TFR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UFFICI> UFFICI { get; set; }
    }
}
