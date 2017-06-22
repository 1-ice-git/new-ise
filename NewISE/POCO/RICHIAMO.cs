namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.RICHIAMO")]
    public partial class RICHIAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RICHIAMO()
        {
            ELAB_CONT = new HashSet<ELAB_CONT>();
            TRASPORTOEFFETTIRIENTRO = new HashSet<TRASPORTOEFFETTIRIENTRO>();
        }

        [Key]
        public decimal IDRICHIAMO { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public decimal IDCOEFINDRICHIAMO { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        public bool RICALCOLATO { get; set; }

        public virtual COEFFICIENTEINDRICHIAMO COEFFICIENTEINDRICHIAMO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELAB_CONT> ELAB_CONT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASPORTOEFFETTIRIENTRO> TRASPORTOEFFETTIRIENTRO { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }
    }
}
