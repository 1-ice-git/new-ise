namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.PRIMASITEMAZIONE")]
    public partial class PRIMASITEMAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRIMASITEMAZIONE()
        {
            ANTICIPI = new HashSet<ANTICIPI>();
            ELAB_CONT = new HashSet<ELAB_CONT>();
            TRASPORTOEFFETTISIST = new HashSet<TRASPORTOEFFETTISIST>();
        }

        [Key]
        public decimal IDPRIMASISTEMAZIONE { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public decimal IDINDSIST { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public bool RICALCOLATA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ANTICIPI> ANTICIPI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }

        public virtual INDENNITASISTEMAZIONE INDENNITASISTEMAZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTISIST> TRASPORTOEFFETTISIST { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
