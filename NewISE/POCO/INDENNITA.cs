namespace NewISE.POCO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ISEPRO.INDENNITA")]
    public partial class INDENNITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public INDENNITA()
        {
            COEFFICIENTESEDE = new HashSet<COEFFICIENTESEDE>();
            INDENNITABASE = new HashSet<INDENNITABASE>();
            LIVELLIDIPENDENTI = new HashSet<LIVELLIDIPENDENTI>();
            MAGGIORAZIONECONIUGE = new HashSet<MAGGIORAZIONECONIUGE>();
            MAGGIORAZIONEFIGLI = new HashSet<MAGGIORAZIONEFIGLI>();
            PERCENTUALEDISAGIO = new HashSet<PERCENTUALEDISAGIO>();
            RUOLODIPENDENTE = new HashSet<RUOLODIPENDENTE>();
            TFR = new HashSet<TFR>();
        }

        [Key]
        public decimal IDTRASFINDENNITA { get; set; }

        public DateTime DATAINIZIO { get; set; }

        public DateTime DATAFINE { get; set; }

        public DateTime DATAAGGIORNAMENTO { get; set; }

        public virtual TRASFERIMENTO TRASFERIMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COEFFICIENTESEDE> COEFFICIENTESEDE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDENNITABASE> INDENNITABASE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LIVELLIDIPENDENTI> LIVELLIDIPENDENTI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONECONIUGE> MAGGIORAZIONECONIUGE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAGGIORAZIONEFIGLI> MAGGIORAZIONEFIGLI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERCENTUALEDISAGIO> PERCENTUALEDISAGIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUOLODIPENDENTE> RUOLODIPENDENTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TFR> TFR { get; set; }
    }
}
