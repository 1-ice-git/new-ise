namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ELAB_CONT")]
    public partial class ELAB_CONT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ELAB_CONT()
        {
            TEORICI = new HashSet<TEORICI>();
        }

        [Key]
        public decimal IDELABCONT { get; set; }

        public decimal? IDINDENNITA { get; set; }

        public decimal? IDANTICIPO { get; set; }

        public decimal? IDRICHIAMO { get; set; }

        public decimal? IDPRIMASISTEMAZIONE { get; set; }

        public virtual ANTICIPI ANTICIPI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }

        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }

        public virtual RICHIAMO RICHIAMO { get; set; }
    }
}
