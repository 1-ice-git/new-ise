namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.REGOLECALCOLO")]
    public partial class REGOLECALCOLO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REGOLECALCOLO()
        {
            RIDUZIONI = new HashSet<RIDUZIONI>();
        }

        [Key]
        public decimal IDREGOLA { get; set; }

        public decimal? IDTIPOREGOLACALCOLO { get; set; }

        public decimal? IDNORMACALCOLO { get; set; }

        [Required]
        [StringLength(1000)]
        public string FORMULAREGOLACALCOLO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual NORMACALCOLO NORMACALCOLO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RIDUZIONI> RIDUZIONI { get; set; }

        public virtual TIPOREGOLACALCOLO TIPOREGOLACALCOLO { get; set; }
    }
}
