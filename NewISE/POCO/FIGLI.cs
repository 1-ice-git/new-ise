namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.FIGLI")]
    public partial class FIGLI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FIGLI()
        {
            ALTRIDATIFAM = new HashSet<ALTRIDATIFAM>();
        }

        [Key]
        public decimal IDFIGLI { get; set; }

        public decimal IDMAGGIORAZIONEFIGLI { get; set; }

        [Required]
        [StringLength(30)]
        public string NOME { get; set; }

        [Required]
        [StringLength(30)]
        public string COGNOME { get; set; }

        [Required]
        [StringLength(16)]
        public string CODICEFISCALE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ALTRIDATIFAM> ALTRIDATIFAM { get; set; }

        public virtual MAGGIORAZIONEFIGLI MAGGIORAZIONEFIGLI { get; set; }
    }
}
