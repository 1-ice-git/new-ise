namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.SOSPENSIONE")]
    public partial class SOSPENSIONE
    {
        [Key]
        public decimal IDSOSPENSIONE { get; set; }

        public decimal IDTIPOSOSPENSIONE { get; set; }

        public decimal IDINDENNITA { get; set; }

        public DateTime DATAINIZIO { get; set; }

        public DateTime DATAFINE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        public virtual TIPOSOSPENSIONE TIPOSOSPENSIONE { get; set; }
    }
}
