namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.UFFICI")]
    public partial class UFFICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UFFICI()
        {
            COEFFICIENTESEDE = new HashSet<COEFFICIENTESEDE>();
            MAGGIORAZIONIANNUALI = new HashSet<MAGGIORAZIONIANNUALI>();
            PERCENTUALEDISAGIO = new HashSet<PERCENTUALEDISAGIO>();
            PERCENTUALEMAB = new HashSet<PERCENTUALEMAB>();
            TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
            COEFFICIENTEFKM = new HashSet<COEFFICIENTEFKM>();
        }

        [Key]
        public decimal IDUFFICIO { get; set; }

        [Required]
        [StringLength(4)]
        public string CODICEUFFICIO { get; set; }

        [Required]
        [StringLength(50)]
        public string DESCRIZIONEUFFICIO { get; set; }

        public decimal IDVALUTA { get; set; }

        public bool PAGATOVALUTAUFFICIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONIANNUALI> MAGGIORAZIONIANNUALI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEMAB> PERCENTUALEMAB { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }

        public virtual VALUTE VALUTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTEFKM> COEFFICIENTEFKM { get; set; }
    }
}
