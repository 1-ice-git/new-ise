namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.VOCI")]
    public partial class VOCI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VOCI()
        {
            TEORICI = new HashSet<TEORICI>();
        }

        [Key]
        public decimal IDVOCI { get; set; }

        public decimal IDTIPOLIQUIDAZIONE { get; set; }

        public decimal IDTIPOVOCE { get; set; }

        [Required]
        [StringLength(30)]
        public string CODICEVOCE { get; set; }

        [Required]
        [StringLength(100)]
        public string DESCRIZIONE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TEORICI> TEORICI { get; set; }

        public virtual TIPOLIQUIDAZIONE TIPOLIQUIDAZIONE { get; set; }

        public virtual TIPOVOCE TIPOVOCE { get; set; }
    }
}
