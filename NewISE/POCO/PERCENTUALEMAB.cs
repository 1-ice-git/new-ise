namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.PERCENTUALEMAB")]
    public partial class PERCENTUALEMAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PERCENTUALEMAB()
        {
            MAGGIORAZIONEABITAZIONE = new HashSet<MAGGIORAZIONEABITAZIONE>();
        }

        [Key]
        public decimal IDPERCMAB { get; set; }

        public decimal IDUFFICIO { get; set; }

        public decimal IDLIVELLO { get; set; }

        public DateTime DATAINIZIOVALIDITA { get; set; }

        public DateTime DATAFINEVALIDITA { get; set; }

        public decimal PERCENTUALE { get; set; }

        public decimal PERCENTUALERESPONSABILE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public bool ANNULLATO { get; set; }

        public virtual LIVELLI LIVELLI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEABITAZIONE> MAGGIORAZIONEABITAZIONE { get; set; }

        public virtual UFFICI UFFICI { get; set; }
    }
}
