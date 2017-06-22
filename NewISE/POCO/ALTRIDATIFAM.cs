namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ALTRIDATIFAM")]
    public partial class ALTRIDATIFAM
    {
        [Key]
        public decimal IDALTRIDATIFAM { get; set; }

        public decimal? IDFIGLI { get; set; }

        public decimal? IDCONIUGE { get; set; }

        public DateTime DATANASCITA { get; set; }

        [Required]
        [StringLength(60)]
        public string COMUNENASCITA { get; set; }

        [Required]
        [StringLength(60)]
        public string PROVINCIANASCITA { get; set; }

        [Required]
        [StringLength(30)]
        public string NAZIONALITA { get; set; }

        [Required]
        [StringLength(100)]
        public string INDIRIZZORESIDENZA { get; set; }

        [Required]
        [StringLength(50)]
        public string COMUNERESIDENZA { get; set; }

        [Required]
        [StringLength(50)]
        public string PROVINCIARESIDENZA { get; set; }

        [Required]
        [StringLength(10)]
        public string CAP { get; set; }

        public decimal STUDENTE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual CONIUGE CONIUGE { get; set; }

        public virtual FIGLI FIGLI { get; set; }
    }
}
