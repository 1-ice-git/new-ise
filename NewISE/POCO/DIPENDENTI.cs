namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.DIPENDENTI")]
    public partial class DIPENDENTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DIPENDENTI()
        {
            LIVELLIDIPENDENTI = new HashSet<LIVELLIDIPENDENTI>();
            TRASFERIMENTO = new HashSet<TRASFERIMENTO>();
        }

        [Key]
        public decimal IDDIPENDENTE { get; set; }

        public int MATRICOLA { get; set; }

        [Required]
        [StringLength(30)]
        public string NOME { get; set; }

        [Required]
        [StringLength(30)]
        public string COGNOME { get; set; }

        public DateTime DATAASSUNZIONE { get; set; }

        public DateTime? DATACESSAZIONE { get; set; }

        [StringLength(100)]
        public string INDIRIZZO { get; set; }

        [StringLength(10)]
        public string CAP { get; set; }

        [StringLength(30)]
        public string CITTA { get; set; }

        [StringLength(30)]
        public string PROVINCIA { get; set; }

        [Required]
        [StringLength(50)]
        public string EMAIL { get; set; }

        [StringLength(30)]
        public string TELEFONO { get; set; }

        [StringLength(30)]
        public string FAX { get; set; }

        public bool ABILITATO { get; set; }

        public DateTime DATAINIZIORICALCOLI { get; set; }

        public virtual CDCGEPE CDCGEPE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRASFERIMENTO> TRASFERIMENTO { get; set; }
    }
}
