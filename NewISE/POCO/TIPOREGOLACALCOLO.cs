namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.TIPOREGOLACALCOLO")]
    public partial class TIPOREGOLACALCOLO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIPOREGOLACALCOLO()
        {
            REGOLECALCOLO = new HashSet<REGOLECALCOLO>();
        }

        [Key]
        public decimal IDTIPOREGOLACALCOLO { get; set; }

        [Required]
        [StringLength(250)]
        public string DESCRIZIONEREGOLA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<REGOLECALCOLO> REGOLECALCOLO { get; set; }
    }
}
