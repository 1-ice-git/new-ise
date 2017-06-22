namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ANTICIPI")]
    public partial class ANTICIPI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ANTICIPI()
        {
            ELAB_CONT = new HashSet<ELAB_CONT>();
        }

        [Key]
        public decimal IDANTICIPO { get; set; }

        public decimal IDPRIMASISTEMAZIONE { get; set; }

        public decimal IMPORTOANTICIPO { get; set; }

        public decimal NOTIFICARICHIESTA { get; set; }

        public decimal PRATICACONCLUSA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }

        public virtual PRIMASITEMAZIONE PRIMASITEMAZIONE { get; set; }
    }
}
