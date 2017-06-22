namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.COEFFICIENTEINDRICHIAMO")]
    public partial class COEFFICIENTEINDRICHIAMO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COEFFICIENTEINDRICHIAMO()
        {
            RICHIAMO = new HashSet<RICHIAMO>();
        }

        [Key]
        public decimal IDCOEFINDRICHIAMO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal COEFFICIENTERICHIAMO { get; set; }

        public decimal COEFFICIENTEINDBASE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RICHIAMO> RICHIAMO { get; set; }
    }
}
