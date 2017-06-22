namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.LOGATTIVITA")]
    public partial class LOGATTIVITA
    {
        [Key]
        public decimal IDLOG { get; set; }

        public decimal IDUTENTELOGGATO { get; set; }

        public decimal? IDTRASFERIMENTO { get; set; }

        public decimal IDATTIVITACRUD { get; set; }

        public DateTime DATAOPERAZIONE { get; set; }

        [Required]
        public string DESCATTIVITASVOLTA { get; set; }

        [StringLength(60)]
        public string TABELLACOINVOLTA { get; set; }

        public decimal? IDTABELLACOINVOLTA { get; set; }

        public virtual ATTIVITACRUD ATTIVITACRUD { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        public virtual UTENTIAUTORIZZATI UTENTIAUTORIZZATI { get; set; }
    }
}
