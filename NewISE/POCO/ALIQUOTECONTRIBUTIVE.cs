namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.ALIQUOTECONTRIBUTIVE")]
    public partial class ALIQUOTECONTRIBUTIVE
    {
        [Key]
        public decimal IDALIQCONTR { get; set; }

        public decimal IDTIPOCONTRIBUTO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal ALIQUOTA { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual MAB_ALIQCONTR MAB_ALIQCONTR { get; set; }

        public virtual TIPOALIQUOTECONTRIBUTIVE TIPOALIQUOTECONTRIBUTIVE { get; set; }
    }
}
