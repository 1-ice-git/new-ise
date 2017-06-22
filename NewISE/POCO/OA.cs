namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.OA")]
    public partial class OA
    {
        [Key]
        public decimal CTB_ID_RECORD { get; set; }

        public short CTB_MATRICOLA { get; set; }

        [Required]
        [StringLength(6)]
        public string CTB_QUALIFICA { get; set; }

        [Required]
        [StringLength(4)]
        public string CTB_COD_SEDE { get; set; }

        [Required]
        [StringLength(3)]
        public string CTB_TIPO_VOCE { get; set; }

        [Required]
        [StringLength(1)]
        public string CTB_TIPO_MOVIMENTO { get; set; }

        [Required]
        [StringLength(50)]
        public string CTB_DESCRIZIONE { get; set; }

        [Required]
        [StringLength(10)]
        public string CTB_COAN { get; set; }

        [Required]
        [StringLength(15)]
        public string CTB_NUM_DOC { get; set; }

        public decimal CTB_IMPORTO { get; set; }

        [Required]
        [StringLength(2)]
        public string CTB_OPER_99 { get; set; }

        public DateTime? CTB_DTINI_MAB { get; set; }

        public DateTime? CTB_DTFIN_MAB { get; set; }

        public short? CTB_GIORNI_MAB { get; set; }

        public DateTime? CTB_DT_CONTABILE { get; set; }

        public virtual CONTABILITA CONTABILITA { get; set; }
    }
}
