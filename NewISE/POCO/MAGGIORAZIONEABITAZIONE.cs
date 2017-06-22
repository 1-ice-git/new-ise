namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.MAGGIORAZIONEABITAZIONE")]
    public partial class MAGGIORAZIONEABITAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAGGIORAZIONEABITAZIONE()
        {
            MAB_DOC = new HashSet<MAB_DOC>();
        }

        [Key]
        public decimal IDMAB { get; set; }

        public decimal IDPERCMAB { get; set; }

        public decimal IDTFR { get; set; }

        public decimal IDMAGANNUALI { get; set; }

        public decimal IDTRASFERIMENTO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public bool ANTICIPOANNUALE { get; set; }

        public bool CONDIVISO { get; set; }

        public bool PAGATOCONDIVISO { get; set; }

        public decimal CANONE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public decimal IDRUOLODIPENDENTE { get; set; }

        public decimal IDLIVDIPENDENTE { get; set; }

        public virtual LIVELLIDIPENDENTI LIVELLIDIPENDENTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAB_DOC> MAB_DOC { get; set; }

        public virtual MAGGIORAZIONIANNUALI MAGGIORAZIONIANNUALI { get; set; }

        public virtual PERCENTUALEMAB PERCENTUALEMAB { get; set; }

        public virtual RUOLODIPENDENTE RUOLODIPENDENTE { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        public virtual TFR TFR { get; set; }
    }
}
